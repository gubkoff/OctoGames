using System;
using NUnit.Framework;
using OctoGames.App.Features.Entities;
using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.Tests
{
    public sealed class GameplayEntityRepositoryTests
    {
        private IRepository<IGameplayEntity> _repository;

        [SetUp]
        public void SetUp() => _repository = new GameplayEntityRepository();

        [Test]
        public void Add_StoresGameplayEntity()
        {
            var entity = CreateEntity(GameplayEntityState.Active);

            _repository.Add(entity);

            Assert.IsTrue(_repository.TryGet(entity.Id, out var found));
            Assert.AreSame(entity, found);
        }

        private static StubGameplayEntity CreateEntity(GameplayEntityState state) =>
            new(Guid.NewGuid(), state);

        private sealed class StubGameplayEntity : IGameplayEntity
        {
            private readonly GameplayEntityData _data;

            public StubGameplayEntity(Guid id, GameplayEntityState state)
            {
                Id = id;
                _data = new GameplayEntityData
                {
                    Type = GameplayEntityType.Enemy,
                    State = state
                };
            }

            public Guid Id { get; }
            public GameplayEntityData Data => _data;
            public bool IsActive => _data.State == GameplayEntityState.Active;
            public GameObject GameObject => null;

            public void Initialize(GameplayEntityData data)
            {
            }

            public void ApplyState(GameplayEntityState state) => _data.State = state;

            public void SyncTransformToData()
            {
            }
        }
    }
}
