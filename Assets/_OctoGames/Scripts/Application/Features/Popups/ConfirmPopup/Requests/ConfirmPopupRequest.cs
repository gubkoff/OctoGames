using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Popups;

namespace OctoGames.App.Features.Popups.ConfirmPopup
{
    public sealed class ConfirmPopupRequest : IPopupRequest
    {
        public ConfirmPopupRequest(
            string title,
            string body,
            string confirmLabel,
            string cancelLabel,
            Func<CancellationToken, UniTask> onConfirm = null,
            Func<CancellationToken, UniTask> onCancel = null)
        {
            Title = title;
            Body = body;
            ConfirmLabel = confirmLabel;
            CancelLabel = cancelLabel;
            OnConfirm = onConfirm;
            OnCancel = onCancel;
        }

        public string Title { get; }
        public string Body { get; }
        public string ConfirmLabel { get; }
        public string CancelLabel { get; }
        public Func<CancellationToken, UniTask> OnConfirm { get; }
        public Func<CancellationToken, UniTask> OnCancel { get; }
    }
}
