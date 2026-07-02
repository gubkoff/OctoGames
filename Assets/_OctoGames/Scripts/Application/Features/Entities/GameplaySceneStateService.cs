using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Persistence;
using OctoGames.Repository;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace OctoGames.App.Features.Entities
{
    public sealed class GameplaySceneStateService : IGameplaySceneStateService
    {
        public const string PersistenceKey = "gameplay_entities_demo";

        private readonly IPersistence _persistence;
        private readonly IRepository<IGameplayEntity> _repository;
        private readonly SOGameplayEntityCatalog _catalog;
        private readonly SOGameplayEntitiesInitialState _initialState;
        private readonly IObjectResolver _resolver;
        private readonly Transform _entitiesRoot;

        public GameplaySceneStateService(
            IPersistence persistence,
            IRepository<IGameplayEntity> repository,
            SOGameplayEntityCatalog catalog,
            SOGameplayEntitiesInitialState initialState,
            IObjectResolver resolver,
            Transform entitiesRoot)
        {
            _persistence = persistence;
            _repository = repository;
            _catalog = catalog;
            _initialState = initialState;
            _resolver = resolver;
            _entitiesRoot = entitiesRoot;
        }

        public async UniTask LoadOrInitializeAsync(CancellationToken ct = default)
        {
            ClearRuntime();

            var entities = await _persistence.ExistsAsync(PersistenceKey, ct)
                ? await _persistence.LoadAsync(PersistenceKey, new List<GameplayEntityData>(), ct)
                : await CreateAndPersistInitialEntitiesAsync(ct);

            SpawnAll(entities);
        }

        public async UniTask SaveAsync(CancellationToken ct = default)
        {
            var entities = _repository.GetAll();
            var data = new List<GameplayEntityData>(entities.Count);

            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (entity?.GameObject == null)
                    continue;

                entity.SyncTransformToData();
                data.Add(entity.Data);
            }

            var result = await _persistence.SaveAsync(PersistenceKey, data, ct);
            if (!result.IsSuccess)
            {
                Debug.LogError(
                    $"Failed to save gameplay entities. Status: {result.Status}, count: {data.Count}");
            }
        }

        public async UniTask ResetToInitialAsync(CancellationToken ct = default)
        {
            ClearRuntime();
            await _persistence.DeleteAsync(PersistenceKey, ct);

            var entities = _initialState.Entities;
            await _persistence.SaveAsync(PersistenceKey, entities, ct);
            SpawnAll(entities);
        }

        public async UniTask AddEntityAsync(GameplayEntityType type, CancellationToken ct = default)
        {
            SpawnEntity(new GameplayEntityData(
                type,
                GameplayEntityState.Active,
                new Vector3(0f, 0.5f, UnityEngine.Random.Range(-3f, 3f))));

            await SaveAsync(ct);
        }

        public void SetEntityState(Guid id, GameplayEntityState state)
        {
            if (_repository.TryGet(id, out var entity))
                entity.ApplyState(state);
        }

        public void DestroyEntity(Guid id)
        {
            if (!_repository.TryGet(id, out var entity))
                return;

            _repository.Remove(entity);

            if (entity.GameObject != null)
                Object.Destroy(entity.GameObject);
        }

        private async UniTask<IReadOnlyList<GameplayEntityData>> CreateAndPersistInitialEntitiesAsync(CancellationToken ct)
        {
            var entities = _initialState.Entities;
            await _persistence.SaveAsync(PersistenceKey, entities, ct);
            return entities;
        }

        private void SpawnAll(IReadOnlyList<GameplayEntityData> entities)
        {
            if (entities == null)
                return;

            for (var i = 0; i < entities.Count; i++)
                SpawnEntity(entities[i]);
        }

        private void SpawnEntity(GameplayEntityData data)
        {
            var prefab = _catalog.GetPrefab(data.Type);
            var instance = Object.Instantiate(prefab, data.Position, Quaternion.Euler(data.RotationEuler), _entitiesRoot);
            _resolver.InjectGameObject(instance);

            var gameplayEntity = instance.GetComponent<GameplayEntityBase>();

            if (gameplayEntity == null)
                throw new InvalidOperationException($"Prefab '{prefab.name}' has no {nameof(GameplayEntityBase)}.");

            gameplayEntity.Initialize(data);
        }

        private void ClearRuntime()
        {
            var entities = _repository.GetAll();
            _repository.Clear();

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i]?.GameObject != null)
                    Object.Destroy(entities[i].GameObject);
            }
        }
    }
}
