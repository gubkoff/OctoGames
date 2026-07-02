using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Persistence;
using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.App.Features.Entities
{
    public sealed class GameplayEntityService : IGameplayEntityService
    {
        public const string PersistenceKey = "gameplay_entities_demo";

        private readonly IPersistence _persistence;
        private readonly IRepository<IGameplayEntity> _repository;
        private readonly IGameplayEntitySpawner _spawner;
        private readonly SOGameplayEntitiesInitialState _initialState;

        public GameplayEntityService(
            IPersistence persistence,
            IRepository<IGameplayEntity> repository,
            IGameplayEntitySpawner spawner,
            SOGameplayEntitiesInitialState initialState)
        {
            _persistence = persistence;
            _repository = repository;
            _spawner = spawner;
            _initialState = initialState;
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

                data.Add(new GameplayEntityData(
                    entity.Type,
                    entity.State,
                    entity.Position,
                    entity.RotationEuler));
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
            _spawner.Spawn(new GameplayEntityData(
                type,
                GameplayEntityState.Active,
                new Vector3(0f, 0.5f, UnityEngine.Random.Range(-3f, 3f))));

            await SaveAsync(ct);
        }

        public void DestroyEntity(Guid id)
        {
            if (!_repository.TryGet(id, out var entity))
                return;

            _spawner.Despawn(entity);
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
                _spawner.Spawn(entities[i]);
        }

        private void ClearRuntime()
        {
            var entities = _repository.GetAll();

            for (var i = 0; i < entities.Count; i++)
                _spawner.Despawn(entities[i]);
        }
    }
}
