using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace OctoGames.Popups
{
    public interface IPopupService
    {
        UniTask ShowAsync<TPopup>(PopupShowOptions? options = null, CancellationToken ct = default)
            where TPopup : PopupBaseView;

        UniTask ShowAsync<TPopup, TRequest>(
            TRequest request,
            PopupShowOptions? options = null,
            CancellationToken ct = default)
            where TPopup : PopupBaseView
            where TRequest : IPopupRequest;

        bool TryGetOpen<TPopup>(out TPopup popup)
            where TPopup : PopupBaseView;

        UniTask CloseAsync<TPopup>(CancellationToken ct = default)
            where TPopup : PopupBaseView;

        UniTask CloseAllAsync(CancellationToken ct = default);

        UniTask CloseAllExceptAsync<TPopup>(CancellationToken ct = default)
            where TPopup : PopupBaseView;

        event Action AllClosed;
    }
}
