using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace OctoGames.App.Features.Popups
{
    public sealed class PopupService : IPopupService
    {
        private readonly IPopupProvider _provider;
        private readonly PopupRoot _root;
        private readonly IObjectResolver _resolver;
        private readonly Queue<PendingShow> _queue = new();

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
                    var pending = new PendingShow(
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

            var viewModel = GetViewModel(instance);
            if (viewModel == null)
            {
                Object.Destroy(instance);
                throw new InvalidOperationException(
                    $"Prefab '{prefab.name}' for '{typeof(TPopup).FullName}' is missing {nameof(IPopupViewModel)}.");
            }

            _active = new ActivePopup(typeof(TPopup), popup);

            try
            {
                popup.RegisterViewModel(viewModel);
                viewModel.Initialize(request, popup);
                await viewModel.ShowAsync(ct);
            }
            finally
            {
                viewModel.Dispose();

                if (instance != null)
                    Object.Destroy(instance);

                _active = null;
                RaiseAllClosedIfNeeded();
                await TryShowNextFromQueueAsync();
            }
        }

        private static IPopupViewModel GetViewModel(GameObject instance)
        {
            var components = instance.GetComponents<MonoBehaviour>();

            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is IPopupViewModel viewModel)
                    return viewModel;
            }

            return null;
        }

        private async UniTask TryShowNextFromQueueAsync()
        {
            while (_queue.Count > 0 && _active == null)
            {
                var next = _queue.Dequeue();

                try
                {
                    await next.Show(next.CancellationToken);
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
            popup.RequestClose();

            await UniTask.WaitUntil(
                () => _active == null || _active.Popup != popup,
                PlayerLoopTiming.Update,
                ct);
        }

        private void CancelQueuedShows(Type exceptType = null)
        {
            if (_queue.Count == 0)
                return;

            var remaining = new Queue<PendingShow>();

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
            public ActivePopup(Type popupType, PopupBaseView popup)
            {
                PopupType = popupType;
                Popup = popup;
            }

            public Type PopupType { get; }
            public PopupBaseView Popup { get; }
        }

        private sealed class PendingShow
        {
            public PendingShow(
                Type popupType,
                Func<CancellationToken, UniTask> show,
                PopupShowOptions? options,
                CancellationToken cancellationToken)
            {
                PopupType = popupType;
                Show = show;
                Options = options;
                CancellationToken = cancellationToken;
                Completion = new UniTaskCompletionSource();
            }

            public Type PopupType { get; }
            public Func<CancellationToken, UniTask> Show { get; }
            public PopupShowOptions? Options { get; }
            public CancellationToken CancellationToken { get; }
            public UniTaskCompletionSource Completion { get; }
        }
    }
}
