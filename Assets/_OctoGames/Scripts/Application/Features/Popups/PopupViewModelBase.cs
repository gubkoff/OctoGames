using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace OctoGames.App.Features.Popups
{
    public abstract class PopupViewModelBase : MonoBehaviour, IPopupViewModel
    {
        protected PopupBaseView View { get; private set; }

        private CompositeDisposable _disposables;
        private UniTaskCompletionSource _closeRequested;
        private bool _initialized;

        public void Initialize<TRequest>(TRequest request, PopupBaseView popup)
            where TRequest : IPopupRequest
        {
            if (_initialized)
                throw new InvalidOperationException($"{GetType().Name} is already initialized.");

            View = popup;
            _disposables = new CompositeDisposable();
            _initialized = true;

            popup.OnDimmerClick(_disposables);
            OnInitialize(request, _disposables);
        }

        protected abstract void OnInitialize<TRequest>(TRequest request, CompositeDisposable disposables)
            where TRequest : IPopupRequest;

        public async UniTask ShowAsync(CancellationToken ct)
        {
            if (!_initialized)
                throw new InvalidOperationException($"{GetType().Name} must be initialized before present.");

            try
            {
                View.SetPhase(PopupPhase.Opening);
                await View.PlayOpenAsync(ct);
                View.SetPhase(PopupPhase.Opened);
                await WaitForCloseAsync(ct);
                await OnCloseStartAsync(ct);
                View.SetPhase(PopupPhase.Closing);
                await View.PlayCloseAsync(ct);
                View.SetPhase(PopupPhase.Closed);
            }
            finally
            {
                Cleanup();
            }
        }

        protected virtual async UniTask WaitForCloseAsync(CancellationToken ct)
        {
            var closeRequested = new UniTaskCompletionSource();
            _closeRequested = closeRequested;

            using (ct.Register(state => ((UniTaskCompletionSource)state).TrySetCanceled(ct), closeRequested))
            {
                await closeRequested.Task;
            }
        }

        protected virtual UniTask OnCloseStartAsync(CancellationToken ct) => UniTask.CompletedTask;

        public void RequestClose() => _closeRequested?.TrySetResult();

        public void Dispose()
        {
            Cleanup();
            OnDispose();
        }

        private void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = null;
            _closeRequested = null;
            View = null;
            _initialized = false;
        }

        protected virtual void OnDispose() { }

        protected virtual void OnDestroy() => Dispose();
    }
}
