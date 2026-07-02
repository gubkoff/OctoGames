using System;

namespace OctoGames.App.Features.Entities
{
    public interface IGameplayEntityStateService
    {
        event Action Changed;

        void SetState(Guid id, GameplayEntityState state);
        bool CanDisable(IGameplayEntity entity);
        bool CanEnable(IGameplayEntity entity);
        bool CanComplete(IGameplayEntity entity);
    }
}
