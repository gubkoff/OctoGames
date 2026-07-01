using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace OctoGames.Popups
{
    public sealed class PopupService : IPopupService
    {
        private readonly IPopupProvider _provider;
        private readonly PopupRoot _root;
        private readonly IObjectResolver _resolver;
        private readonly Queue<PendingPopup> _queue = new();

        private ActivePopup _active;

        public PopupService(
            IPopupProvider provider,
            PopupRoot root,
            IObjectResolver resolver)
        {
            _provider = provider;
            _root = root;
            _resolver = resolver;
        }

        public event Action AllClosed;

        public UniTask ShowAsync<TPopup>(PopupShowOptions? options = null, CancellationToken ct = default)
            where TPopup : PopupBaseView
        {
            return ShowAsync<TPopup, EmptyPopupRequest>(EmptyPopupRequest.Instance, options, ct);
        }

        public async UniTask ShowAsync<TPopup, TRequest>(
            TRequest request,
            PopupShowOptions? options = null,
            CancellationToken ct = default)
            where TPopup : PopupBaseView
            where TRequest : IPopupRequest
        {
            var policy = ResolvePolicy(typeof(TPopup), options);

            if (_active != null)
            {
                if (policy == PopupShowPolicy.ReuseIfOpen && _active.PopupType == typeof(TPopup))
                    return;

                if (policy == PopupShowPolicy.Replace && _active.PopupType == typeof(TPopup))
                    await CloseActiveAsync(ct);

                if (_active != null && policy == PopupShowPolicy.Queue)
                {
                    var pending = new PendingPopup(
                        typeof(TPopup),
                        ct => ShowAsync<TPopup, TRequest>(request, ct),
                        options,
                        ct);
                    _queue.Enqueue(pending);
                    await pending.Completion.Task.AttachExternalCancellation(ct);
                    return;
                }
            }

            await ShowAsync<TPopup, TRequest>(request, ct);
        }

        public bool TryGetOpen<TPopup>(out TPopup popup)
            where TPopup : PopupBaseView
        {
            if (_active != null
                && _active.PopupType == typeof(TPopup)
                && _active.Popup.Phase != PopupPhase.Closed)
            {
                popup = (TPopup)_active.Popup;
                return true;
            }

            popup = null;
            return false;
        }

        public async UniTask CloseAsync<TPopup>(CancellationToken ct = default)
            where TPopup : PopupBaseView
        {
            if (_active == null || _active.PopupType != typeof(TPopup))
                return;

            await CloseActiveAsync(ct);
        }

        public async UniTask CloseAllAsync(CancellationToken ct = default)
        {
            CancelQueuedShows();

            if (_active != null)
                await CloseActiveAsync(ct);

            RaiseAllClosedIfNeeded();
        }

        public async UniTask CloseAllExceptAsync<TPopup>(CancellationToken ct = default)
            where TPopup : PopupBaseView
        {
            var exceptType = typeof(TPopup);

            CancelQueuedShows(exceptType);

            if (_active != null && _active.PopupType != exceptType)
                await CloseActiveAsync(ct);

            RaiseAllClosedIfNeeded();
        }

        private async UniTask ShowAsync<TPopup, TRequest>(TRequest request, CancellationToken ct)
            where TPopup : PopupBaseView
            where TRequest : IPopupRequest
        {
            var prefab = _provider.GetPrefab<TPopup>();
            var instance = Object.Instantiate(prefab, _root.Container);
            _resolver.InjectGameObject(instance);

            var popup = instance.GetComponent<TPopup>();
            if (popup == null)
            {
                Object.Destroy(instance);
                throw new InvalidOperationException(
                    $"Prefab '{prefab.name}' for '{typeof(TPopup).FullName}' is missing {nameof(TPopup)}.");
            }

            var binder = GetBinder<TPopup>(instance);
            var viewModel = binder.ResolveViewModel(_resolver);

            _active = new ActivePopup(typeof(TPopup), popup, viewModel);
            var disposables = new CompositeDisposable();

            try
            {
                await viewModel.InitializeAsync(request, ct);
                binder.Bind(popup, viewModel, disposables);

                popup.SetPhase(PopupPhase.Opening);
                await popup.PlayOpenAsync(ct);
                popup.SetPhase(PopupPhase.Opened);
                await viewModel.WaitForCloseAsync(ct);
                await viewModel.OnCloseAsync(ct);
                popup.SetPhase(PopupPhase.Closing);
                await popup.PlayCloseAsync(ct);
                popup.SetPhase(PopupPhase.Closed);
            }
            finally
            {
                disposables.Dispose();
                viewModel.Dispose();

                if (instance != null)
                    Object.Destroy(instance);

                _active = null;
                RaiseAllClosedIfNeeded();
                await TryShowNextFromQueueAsync();
            }
        }

        private static IPopupBinder<TPopup> GetBinder<TPopup>(GameObject instance)
            where TPopup : PopupBaseView
        {
            var components = instance.GetComponents<MonoBehaviour>();

            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is IPopupBinder<TPopup> binder)
                    return binder;
            }

            throw new InvalidOperationException(
                $"Prefab '{instance.name}' for '{typeof(TPopup).FullName}' is missing {nameof(IPopupBinder<TPopup>)}.");
        }

        private async UniTask TryShowNextFromQueueAsync()
        {
            while (_queue.Count > 0 && _active == null)
            {
                var next = _queue.Dequeue();

                try
                {
                    await next.Popup(next.CancellationToken);
                    next.Completion.TrySetResult();
                }
                catch (Exception exception)
                {
                    next.Completion.TrySetException(exception);
                    throw;
                }
            }
        }

        private async UniTask CloseActiveAsync(CancellationToken ct)
        {
            var popup = _active.Popup;

            if (popup.Phase == PopupPhase.Opened)
                _active.ViewModel.RequestClose();

            await UniTask.WaitUntil(
                () => _active == null || _active.Popup != popup,
                PlayerLoopTiming.Update,
                ct);
        }

        private void CancelQueuedShows(Type exceptType = null)
        {
            if (_queue.Count == 0)
                return;

            var remaining = new Queue<PendingPopup>();

            while (_queue.Count > 0)
            {
                var pending = _queue.Dequeue();
                if (exceptType != null && pending.PopupType == exceptType)
                {
                    remaining.Enqueue(pending);
                    continue;
                }

                pending.Completion.TrySetCanceled();
            }

            while (remaining.Count > 0)
                _queue.Enqueue(remaining.Dequeue());
        }

        private PopupShowPolicy ResolvePolicy(Type popupType, PopupShowOptions? options)
        {
            if (options.HasValue)
                return options.Value.Policy;

            return _provider.GetShowPolicy(popupType);
        }

        private void RaiseAllClosedIfNeeded()
        {
            if (_active == null && _queue.Count == 0)
                AllClosed?.Invoke();
        }

        private sealed class ActivePopup
        {
            public ActivePopup(Type popupType, PopupBaseView popup, IPopupViewModel viewModel)
            {
                PopupType = popupType;
                Popup = popup;
                ViewModel = viewModel;
            }

            public Type PopupType { get; }
            public PopupBaseView Popup { get; }
            public IPopupViewModel ViewModel { get; }
        }

        private sealed class PendingPopup
        {
            public PendingPopup(
                Type popupType,
                Func<CancellationToken, UniTask> popup,
                PopupShowOptions? options,
                CancellationToken cancellationToken)
            {
                PopupType = popupType;
                Popup = popup;
                Options = options;
                CancellationToken = cancellationToken;
                Completion = new UniTaskCompletionSource();
            }

            public Type PopupType { get; }
            public Func<CancellationToken, UniTask> Popup { get; }
            public PopupShowOptions? Options { get; }
            public CancellationToken CancellationToken { get; }
            public UniTaskCompletionSource Completion { get; }
        }
    }
}
