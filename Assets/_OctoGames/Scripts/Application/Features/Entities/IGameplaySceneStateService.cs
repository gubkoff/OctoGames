using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplaySceneStateService
    {
        UniTask LoadOrInitializeAsync(CancellationToken ct = default);
        UniTask SaveAsync(CancellationToken ct = default);
        UniTask ResetToInitialAsync(CancellationToken ct = default);
        UniTask AddEntityAsync(GameplayEntityType type, CancellationToken ct = default);
        void SetEntityState(Guid id, GameplayEntityState state);
        void DestroyEntity(Guid id);
    }
}
