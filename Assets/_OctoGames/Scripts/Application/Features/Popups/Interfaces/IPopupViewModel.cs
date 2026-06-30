using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace OctoGames.App.Features.Popups
{
    public interface IPopupViewModel : IDisposable
    {
        UniTask InitializeAsync<TRequest>(TRequest request, CancellationToken ct)
            where TRequest : IPopupRequest;

        UniTask WaitForCloseAsync(CancellationToken ct);

        UniTask OnCloseAsync(CancellationToken ct);

        void RequestClose();
    }
}
