using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplayEntity : IRepositoryEntity
    {
        GameplayEntityData Data { get; }
        bool IsActive { get; }
        GameObject GameObject { get; }
        void Initialize(GameplayEntityData data);
        void ApplyState(GameplayEntityState state);
        void SyncTransformToData();
    }
}
