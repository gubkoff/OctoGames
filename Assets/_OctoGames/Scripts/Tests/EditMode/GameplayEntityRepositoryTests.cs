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
            private GameplayEntityState _state;

            public StubGameplayEntity(Guid id, GameplayEntityState state)
            {
                Id = id;
                _state = state;
            }

            public Guid Id { get; }
            public GameplayEntityType Type => GameplayEntityType.Enemy;
            public GameplayEntityState State => _state;
            public Vector3 Position { get; set; }
            public Vector3 RotationEuler { get; set; }
            public bool IsActive => _state == GameplayEntityState.Active;
            public GameObject GameObject => null;

            public void Initialize(GameplayEntityData data)
            {
            }

            public void ApplyState(GameplayEntityState state) => _state = state;
        }
    }
}
