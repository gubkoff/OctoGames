using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OctoGames.Composition;
using OctoGames.Gameplay.Features.Settings;
using VContainer;

namespace OctoGames.Tests
{
    public sealed class SettingsTests
    {
        string _tempRoot;

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
            var settings = CreateSettingsService();

            settings.Current.SoundVolume = 75;
            settings.Current.MusicVolume = 40;
            settings.Current.PushNotificationsEnabled = true;

            var saveResult = await settings.SaveAsync();
            Assert.IsTrue(saveResult.IsSuccess);

            await settings.LoadAsync();

            Assert.AreEqual(75, settings.Current.SoundVolume);
            Assert.AreEqual(40, settings.Current.MusicVolume);
            Assert.IsTrue(settings.Current.PushNotificationsEnabled);
        }

        [Test]
        public async Task Load_UsesDefaults_WhenMissing()
        {
            var settings = CreateSettingsService();

            await settings.LoadAsync();

            Assert.AreEqual(GameSettings.MaxVolume, settings.Current.SoundVolume);
            Assert.AreEqual(GameSettings.MaxVolume, settings.Current.MusicVolume);
            Assert.IsFalse(settings.Current.PushNotificationsEnabled);
        }

        [Test]
        public async Task Save_ClampsVolume()
        {
            var settings = CreateSettingsService();

            settings.Current.SoundVolume = 200;
            settings.Current.MusicVolume = 0;

            await settings.SaveAsync();
            await settings.LoadAsync();

            Assert.AreEqual(GameSettings.MaxVolume, settings.Current.SoundVolume);
            Assert.AreEqual(GameSettings.MinVolume, settings.Current.MusicVolume);
        }

        ISettingsService CreateSettingsService()
        {
            var builder = new ContainerBuilder();
            PersistenceRegistration.RegisterWithRoot(builder, _tempRoot);
            SettingsRegistration.Register(builder);
            return builder.Build().Resolve<ISettingsService>();
        }
    }
}
