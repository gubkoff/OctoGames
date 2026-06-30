using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace OctoGames.App.Features.Popups
{
    public abstract class PopupViewModelBase : IPopupViewModel
    {
        private UniTaskCompletionSource _closeRequested;

        public UniTask InitializeAsync<TRequest>(TRequest request, CancellationToken ct)
            where TRequest : IPopupRequest
        {
            return OnInitializeAsync(request, ct);
        }

        protected virtual UniTask OnInitializeAsync<TRequest>(TRequest request, CancellationToken ct)
            where TRequest : IPopupRequest
        {
            return UniTask.CompletedTask;
        }

        public async UniTask WaitForCloseAsync(CancellationToken ct)
        {
            var closeRequested = new UniTaskCompletionSource();
            _closeRequested = closeRequested;

            using (ct.Register(state => ((UniTaskCompletionSource)state).TrySetCanceled(ct), closeRequested))
            {
                await closeRequested.Task;
            }
        }

        public virtual UniTask OnCloseAsync(CancellationToken ct) => UniTask.CompletedTask;

        public void RequestClose() => _closeRequested?.TrySetResult();

        public void Dispose()
        {
            _closeRequested = null;
            OnDispose();
        }

        protected virtual void OnDispose() { }
    }
}
