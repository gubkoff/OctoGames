using System;
using NUnit.Framework;
using OctoGames.App.Features.Entities;
using OctoGames.Repository;
using UnityEngine;

namespace OctoGames.Tests
{
    public sealed class GameplayEntityTrackerTests
    {
        private IRepository<IGameplayEntity> _repository;
        private GameplayEntityTracker _tracker;

        [SetUp]
        public void SetUp()
        {
            _repository = new GameplayEntityRepository();
            _tracker = new GameplayEntityTracker(_repository);
        }

        [Test]
        public void GetActiveEntitiesCount_ReturnsOnlyActive()
        {
            _repository.Add(CreateEntity(GameplayEntityState.Active));
            _repository.Add(CreateEntity(GameplayEntityState.Disabled));
            _repository.Add(CreateEntity(GameplayEntityState.Completed));

            Assert.AreEqual(1, _tracker.GetActiveEntitiesCount<IGameplayEntity>());
        }

        [Test]
        public void GetActiveEntitiesCount_Generic_ReturnsTypedActiveCount()
        {
            _repository.Add(CreateEntity(GameplayEntityState.Active));
            _repository.Add(CreateEntity(GameplayEntityState.Disabled));

            Assert.AreEqual(1, _tracker.GetActiveEntitiesCount<StubGameplayEntity>());
        }

        [Test]
        public void GetActiveEntities_ReturnsOnlyActive()
        {
            var active = CreateEntity(GameplayEntityState.Active);
            var disabled = CreateEntity(GameplayEntityState.Disabled);
            var completed = CreateEntity(GameplayEntityState.Completed);

            _repository.Add(active);
            _repository.Add(disabled);
            _repository.Add(completed);

            var result = _tracker.GetActiveEntities();

            Assert.AreEqual(1, result.Count);
            Assert.AreSame(active, result[0]);
        }

        [Test]
        public void GetActiveEntities_Generic_ReturnsTypedActiveEntities()
        {
            _repository.Add(CreateEntity(GameplayEntityState.Active));

            Assert.AreEqual(1, _tracker.GetActiveEntities<StubGameplayEntity>().Count);
        }

        [Test]
        public void GetActiveEntities_WhenEmpty_ReturnsEmptyArray()
        {
            Assert.AreEqual(0, _tracker.GetActiveEntities().Count);
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
