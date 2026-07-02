using System.Collections.Generic;
using OctoGames.Repository;

namespace OctoGames.App.Features.Entities
{
    public sealed class GameplayEntityTracker : IGameplayEntityTracker
    {
        private readonly IRepository<IGameplayEntity> _repository;

        public GameplayEntityTracker(IRepository<IGameplayEntity> repository)
        {
            _repository = repository;
        }

        public int GetActiveEntitiesCount<T>() where T : class, IGameplayEntity
        {
            var all = _repository.GetAll();
            var activeCount = 0;

            for (var i = 0; i < all.Count; i++)
            {
                var entity = all[i];
                if (entity is T typed && typed.IsActive)
                    activeCount++;
            }

            return activeCount;
        }

        public IReadOnlyList<T> GetActiveEntities<T>() where T : class, IGameplayEntity
        {
            var all = _repository.GetAll();
            var activeCount = GetActiveEntitiesCount<T>();

            if (activeCount == 0)
                return System.Array.Empty<T>();

            var result = new T[activeCount];
            var index = 0;

            for (var i = 0; i < all.Count; i++)
            {
                if (all[i] is T typed && typed.IsActive)
                    result[index++] = typed;
            }

            return result;
        }

        public IReadOnlyList<IGameplayEntity> GetActiveEntities() =>
            GetActiveEntities<IGameplayEntity>();
    }
}
