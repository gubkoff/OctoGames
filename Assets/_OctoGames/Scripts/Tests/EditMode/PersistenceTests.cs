using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OctoGames.Composition;
using OctoGames.Persistence;
using VContainer;

namespace OctoGames.Tests
{
    public sealed class PersistenceTests
    {
        private sealed class TestSaveData
        {
            public int Score { get; set; }
        }

        private string _tempRoot;

        [SetUp]
        public void SetUp()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), "OctoGamesTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempRoot);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempRoot))
                Directory.Delete(_tempRoot, recursive: true);
        }

        [Test]
        public async Task SaveAndLoad_RoundTrip()
        {
            var persistence = CreatePersistence();

            var saveResult = await persistence.SaveAsync("player", new TestSaveData { Score = 42 });
            Assert.IsTrue(saveResult.IsSuccess);

            var loaded = await persistence.LoadAsync<TestSaveData>("player");
            Assert.AreEqual(42, loaded.Score);
        }

        [Test]
        public async Task TryLoad_ReturnsNotFound_WhenMissing()
        {
            var persistence = CreatePersistence();

            var result = await persistence.TryLoadAsync<TestSaveData>("missing");

            Assert.AreEqual(PersistenceStatus.NotFound, result.Status);
        }

        [Test]
        public async Task Delete_RemovesFile()
        {
            var persistence = CreatePersistence();
            await persistence.SaveAsync("slot", new TestSaveData { Score = 1 });

            var deleteResult = await persistence.DeleteAsync("slot");
            Assert.IsTrue(deleteResult.IsSuccess);
            Assert.IsFalse(await persistence.ExistsAsync("slot"));
        }

        private IPersistence CreatePersistence()
        {
            var builder = new ContainerBuilder();
            PersistenceRegistration.RegisterWithRoot(builder, _tempRoot);
            return builder.Build().Resolve<IPersistence>();
        }
    }
}
