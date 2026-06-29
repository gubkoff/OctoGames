using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace OctoGames.App.Features.Popups
{
    public interface IPopupViewModel : IDisposable
    {
        void Initialize<TRequest>(TRequest request, PopupBaseView popup)
            where TRequest : IPopupRequest;

        UniTask ShowAsync(CancellationToken ct);

        void RequestClose();
    }
}
