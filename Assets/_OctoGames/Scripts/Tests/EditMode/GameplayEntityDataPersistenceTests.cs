using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OctoGames.App.Features.Entities;
using OctoGames.Composition;
using OctoGames.Persistence;
using UnityEngine;
using VContainer;

namespace OctoGames.Tests
{
    public sealed class GameplayEntityDataPersistenceTests
    {
        private string _tempRoot;

        [SetUp]
        public void SetUp()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), "OctoGamesTests", System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRoot);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempRoot))
                Directory.Delete(_tempRoot, recursive: true);
        }

        [Test]
        public async Task SaveAndLoad_RoundTripsEntityList_WithVector3()
        {
            var persistence = CreatePersistence();
            var entities = new List<GameplayEntityData>
            {
                new(GameplayEntityType.Enemy, GameplayEntityState.Active, new Vector3(1f, 0.5f, 2f)),
                new(GameplayEntityType.Interactable, GameplayEntityState.Disabled, new Vector3(-1f, 0.5f, 0f))
            };

            var saveResult = await persistence.SaveAsync(GameplayEntityService.PersistenceKey, entities);
            Assert.IsTrue(saveResult.IsSuccess);

            var loaded = await persistence.LoadAsync(
                GameplayEntityService.PersistenceKey,
                new List<GameplayEntityData>());

            Assert.AreEqual(2, loaded.Count);
            Assert.AreEqual(GameplayEntityType.Enemy, loaded[0].Type);
            Assert.AreEqual(GameplayEntityState.Disabled, loaded[1].State);
            Assert.AreEqual(new Vector3(1f, 0.5f, 2f), loaded[0].Position);
            Assert.AreEqual(new Vector3(-1f, 0.5f, 0f), loaded[1].Position);
        }

        private IPersistence CreatePersistence()
        {
            var builder = new ContainerBuilder();
            PersistenceRegistration.RegisterWithRoot(builder, _tempRoot);
            return builder.Build().Resolve<IPersistence>();
        }
    }
}
