using System.Collections.Generic;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    [CreateAssetMenu(
        fileName = "GameplayEntitiesInitialState",
        menuName = "OctoGames/Gameplay/Entities Initial State")]
    public sealed class SOGameplayEntitiesInitialState : ScriptableObject
    {
        [SerializeField] private GameplayEntityData[] _entities =
        {
            new(GameplayEntityType.Enemy, GameplayEntityState.Active, new Vector3(-2f, 0.5f, 0f)),
            new(GameplayEntityType.Interactable, GameplayEntityState.Active, new Vector3(0f, 0.5f, 2f)),
            new(GameplayEntityType.StoryActor, GameplayEntityState.Disabled, new Vector3(2f, 0.5f, -1f))
        };

        public IReadOnlyList<GameplayEntityData> Entities => _entities;
    }
}
