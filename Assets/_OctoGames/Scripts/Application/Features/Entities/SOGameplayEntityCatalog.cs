using System;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    [Serializable]
    public struct GameplayEntityCatalogEntry
    {
        public GameplayEntityType Type;
        public GameplayEntityBase Prefab;
    }

    [CreateAssetMenu(
        fileName = "GameplayEntityCatalog",
        menuName = "OctoGames/Gameplay/Entity Catalog")]
    public sealed class SOGameplayEntityCatalog : ScriptableObject
    {
        [SerializeField] private GameplayEntityCatalogEntry[] _entries;

        public GameObject GetPrefab(GameplayEntityType type)
        {
            if (_entries == null)
                throw new InvalidOperationException("Entity catalog has no entries.");

            foreach (var entry in _entries)
            {
                if (entry.Type == type && entry.Prefab != null)
                    return entry.Prefab.gameObject;
            }

            throw new InvalidOperationException($"Prefab for entity type '{type}' is not configured.");
        }
    }
}
