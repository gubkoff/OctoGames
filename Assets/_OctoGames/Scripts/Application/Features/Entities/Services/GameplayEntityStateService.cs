using System;
using OctoGames.Repository;

namespace OctoGames.App.Features.Entities
{
    public sealed class GameplayEntityStateService : IGameplayEntityStateService
    {
        private readonly IRepository<IGameplayEntity> _repository;

        public GameplayEntityStateService(IRepository<IGameplayEntity> repository)
        {
            _repository = repository;
        }

        public void SetState(Guid id, GameplayEntityState state)
        {
            if (_repository.TryGet(id, out var entity))
                entity.ApplyState(state);
        }

        public bool CanDisable(IGameplayEntity entity) =>
            entity.State == GameplayEntityState.Active;

        public bool CanEnable(IGameplayEntity entity) =>
            entity.State == GameplayEntityState.Disabled;

        public bool CanComplete(IGameplayEntity entity) =>
            entity.State == GameplayEntityState.Active &&
            (entity.Type == GameplayEntityType.Interactable ||
             entity.Type == GameplayEntityType.StoryActor);
    }
}
