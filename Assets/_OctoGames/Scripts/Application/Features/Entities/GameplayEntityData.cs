using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    [Serializable]
    public sealed class GameplayEntityData
    {
        public GameplayEntityType Type;
        public GameplayEntityState State;
        public Vector3 Position;
        public Vector3 RotationEuler;

        public GameplayEntityData()
        {
        }

        public GameplayEntityData(
            GameplayEntityType type,
            GameplayEntityState state,
            Vector3 position,
            Vector3 rotationEuler = default)
        {
            Type = type;
            State = state;
            Position = position;
            RotationEuler = rotationEuler;
        }
    }
}
