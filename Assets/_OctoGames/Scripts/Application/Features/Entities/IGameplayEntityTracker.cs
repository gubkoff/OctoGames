using System.Collections.Generic;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplayEntityTracker
    {
        int GetActiveEntitiesCount<T>() where T : class, IGameplayEntity;
        IReadOnlyList<T> GetActiveEntities<T>() where T : class, IGameplayEntity;
        IReadOnlyList<IGameplayEntity> GetActiveEntities();
    }
}
