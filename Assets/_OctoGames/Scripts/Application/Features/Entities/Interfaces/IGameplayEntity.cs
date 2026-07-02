using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplayEntity : IRepositoryEntity
    {
        GameplayEntityType Type { get; }
        GameplayEntityState State { get; }
        Vector3 Position { get; set; }
        Vector3 RotationEuler { get; set; }
        bool IsActive { get; }
        GameObject GameObject { get; }
        void Initialize(GameplayEntityData data);
        void ApplyState(GameplayEntityState state);
    }
}
