namespace OctoGames.App.Features.Entities
{
    public static class GameplayEntityTransitions
    {
        public static bool CanDisable(IGameplayEntity entity) =>
            entity.Data.State == GameplayEntityState.Active;

        public static bool CanEnable(IGameplayEntity entity) =>
            entity.Data.State == GameplayEntityState.Disabled;

        public static bool CanComplete(IGameplayEntity entity) =>
            entity.Data.State == GameplayEntityState.Active &&
            (entity.Data.Type == GameplayEntityType.Interactable ||
             entity.Data.Type == GameplayEntityType.StoryActor);
    }
}
