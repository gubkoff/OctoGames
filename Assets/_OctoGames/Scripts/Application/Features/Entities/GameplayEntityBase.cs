using System;
using OctoGames.Repository;
using UnityEngine;
using VContainer;

namespace OctoGames.App.Features.Entities
{
    public abstract class GameplayEntityBase : MonoBehaviour, IGameplayEntity
    {
        private Guid _id;
        private GameplayEntityData _data = new();
        private IRepository<IGameplayEntity> _repository;

        public Guid Id => _id;
        public GameplayEntityData Data => _data;
        public bool IsActive => _data.State == GameplayEntityState.Active;
        public GameObject GameObject => gameObject;

        [Inject]
        private void Construct(IRepository<IGameplayEntity> repository)
        {
            _repository = repository;
        }

        public void Initialize(GameplayEntityData data)
        {
            if (_repository == null)
                throw new InvalidOperationException($"{nameof(IGameplayEntity)} was not injected before Initialize.");

            _id = Guid.NewGuid();
            _data = new GameplayEntityData(
                data.Type,
                data.State,
                data.Position,
                data.RotationEuler);
            _repository.Add(this);
        }

        public void ApplyState(GameplayEntityState state)
        {
            _data.State = state;
        }

        public void SyncTransformToData()
        {
            _data.Position = transform.position;
            _data.RotationEuler = transform.eulerAngles;
        }

        protected virtual void OnDestroy()
        {
            if (_repository != null && _id != Guid.Empty)
                _repository.Remove(this);
        }
    }
}
