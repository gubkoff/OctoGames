using System.Collections.Generic;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    [CreateAssetMenu(
        fileName = "GameplayEntitiesInitialState",
        menuName = "OctoGames/Gameplay/Entities Initial State")]
    public sealed class SOGameplayEntitiesInitialState : ScriptableObject
    {
        [SerializeField] private GameplayEntityData[] _entities;

        public IReadOnlyList<GameplayEntityData> Entities => _entities;
    }
}