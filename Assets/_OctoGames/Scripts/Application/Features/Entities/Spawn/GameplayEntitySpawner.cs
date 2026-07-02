using System;
using OctoGames.Repository;
using OctoGames.Spawn;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace OctoGames.App.Features.Entities
{
    public sealed class GameplayEntitySpawner : BaseSpawner<IGameplayEntity, GameplayEntityData>, IGameplayEntitySpawner
    {
        private readonly SOGameplayEntityCatalog _catalog;
        private readonly IObjectResolver _resolver;
        private readonly Transform _entitiesRoot;
        private readonly IRepository<IGameplayEntity> _repository;

        public GameplayEntitySpawner(
            SOGameplayEntityCatalog catalog,
            IObjectResolver resolver,
            Transform entitiesRoot,
            IRepository<IGameplayEntity> repository)
        {
            _catalog = catalog;
            _resolver = resolver;
            _entitiesRoot = entitiesRoot;
            _repository = repository;
        }

        public override IGameplayEntity Spawn(GameplayEntityData data)
        {
            var prefab = _catalog.GetPrefab(data.Type);
            var instance = Object.Instantiate(prefab, data.Position, Quaternion.Euler(data.RotationEuler), _entitiesRoot);
            _resolver.InjectGameObject(instance);

            var gameplayEntity = instance.GetComponent<GameplayEntityBase>();
            if (gameplayEntity == null)
                throw new InvalidOperationException($"Prefab '{prefab.name}' has no {nameof(GameplayEntityBase)}.");

            gameplayEntity.Initialize(data);
            _repository.Add(gameplayEntity);
            return gameplayEntity;
        }

        protected override void OnDespawn(IGameplayEntity entity)
        {
            _repository.Remove(entity);

            if (entity.GameObject != null)
                Object.Destroy(entity.GameObject);
        }
    }
}
