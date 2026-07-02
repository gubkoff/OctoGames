using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Popups;
using R3;
using VContainer;

namespace OctoGames.App.Features.Popups.ConfirmPopup
{
    public sealed class ConfirmPopupViewModel : PopupViewModelBase
    {
        public ReactiveProperty<string> Title { get; } = new();
        public ReactiveProperty<string> Body { get; } = new();
        public ReactiveProperty<string> ConfirmLabel { get; } = new();
        public ReactiveProperty<string> CancelLabel { get; } = new();

        private ConfirmPopupRequest _request;
        private bool _confirmed;

        protected override UniTask OnInitializeAsync<TRequest>(TRequest request, CancellationToken ct)
        {
            _request = (ConfirmPopupRequest)(object)request;
            Title.Value = _request.Title;
            Body.Value = _request.Body;
            ConfirmLabel.Value = _request.ConfirmLabel;
            CancelLabel.Value = _request.CancelLabel;
            return UniTask.CompletedTask;
        }

        public void Confirm()
        {
            _confirmed = true;
            RequestClose();
        }

        public void Cancel()
        {
            _confirmed = false;
            RequestClose();
        }

        public override async UniTask OnCloseAsync(CancellationToken ct)
        {
            if (_request == null)
                return;

            if (_confirmed && _request.OnConfirm != null)
                await _request.OnConfirm(ct);
            else if (!_confirmed && _request.OnCancel != null)
                await _request.OnCancel(ct);
        }

        protected override void OnDispose()
        {
            Title.Dispose();
            Body.Dispose();
            ConfirmLabel.Dispose();
            CancelLabel.Dispose();
        }
    }
}
