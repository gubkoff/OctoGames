using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using VContainer.Unity;

namespace OctoGames.App
{
    public sealed class ApplicationEntryPoint : IAsyncStartable
    {
        private readonly IGameplayEntityService _entityService;

        public ApplicationEntryPoint(IGameplayEntityService entityService)
        {
            _entityService = entityService;
        }

        public UniTask StartAsync(CancellationToken ct) => _entityService.LoadOrInitializeAsync(ct);
    }
}
