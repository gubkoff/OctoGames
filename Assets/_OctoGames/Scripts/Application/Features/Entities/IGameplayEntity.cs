using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplayEntity : IRepositoryEntity
    {
        GameplayEntityType Type { get; }
        GameplayEntityState State { get; }
        bool IsActive { get; }
        GameObject GameObject { get; }
    }
}
