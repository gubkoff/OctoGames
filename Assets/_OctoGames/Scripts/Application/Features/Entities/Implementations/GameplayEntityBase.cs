using System;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    public abstract class GameplayEntityBase : MonoBehaviour, IGameplayEntity
    {
        private Guid _id;
        private GameplayEntityState _state = GameplayEntityState.Active;

        public Guid Id => _id;
        public abstract GameplayEntityType Type { get; }
        public GameplayEntityState State => _state;
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public Vector3 RotationEuler
        {
            get => transform.eulerAngles;
            set => transform.rotation = Quaternion.Euler(value);
        }
        public bool IsActive => _state == GameplayEntityState.Active;
        public GameObject GameObject => gameObject;

        public void Initialize(GameplayEntityData data)
        {
            _id = Guid.NewGuid();
            _state = data.State;
            Position = data.Position;
            RotationEuler = data.RotationEuler;
        }

        public void ApplyState(GameplayEntityState state)
        {
            _state = state;
        }
    }
}
