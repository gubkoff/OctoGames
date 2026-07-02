using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using VContainer.Unity;

namespace OctoGames.App
{
    public sealed class ApplicationEntryPoint : IAsyncStartable
    {
        private readonly IGameplaySceneStateService _sceneState;

        public ApplicationEntryPoint(IGameplaySceneStateService sceneState)
        {
            _sceneState = sceneState;
        }

        public UniTask StartAsync(CancellationToken ct) => _sceneState.LoadOrInitializeAsync(ct);
    }
}
