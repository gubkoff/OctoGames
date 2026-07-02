using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using OctoGames.Popups;
using OctoGames.App.Features.Settings;
using OctoGames.App.Features.Popups.SettingsPopup;
using OctoGames.Composition;
using VContainer;

namespace OctoGames.Tests
{
    public sealed class SettingsPopupViewModelTests
    {
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
        public async Task Initialize_LoadsPersistedSettings()
        {
            var container = CreateContainer();
            var settings = container.Resolve<ISettingsService>();
            settings.Current.SoundVolume = 42;
            settings.Current.MusicVolume = 17;
            settings.Current.PushNotificationsEnabled = true;
            await settings.SaveAsync();

            var viewModel = CreateViewModel(container);
            await viewModel.InitializeAsync(EmptyPopupRequest.Instance, CancellationToken.None);

            Assert.AreEqual(42, viewModel.SoundVolume.Value);
            Assert.AreEqual(17, viewModel.MusicVolume.Value);
            Assert.IsTrue(viewModel.PushNotificationsEnabled.Value);
        }

        [Test]
        public async Task OnCloseAsync_PersistsChanges()
        {
            var container = CreateContainer();
            var settings = container.Resolve<ISettingsService>();
            await settings.LoadAsync();

            var viewModel = CreateViewModel(container);
            await viewModel.InitializeAsync(EmptyPopupRequest.Instance, CancellationToken.None);
            viewModel.SoundVolume.Value = 55;
            viewModel.MusicVolume.Value = 33;
            viewModel.PushNotificationsEnabled.Value = true;

            await viewModel.OnCloseAsync(CancellationToken.None);

            Assert.AreEqual(55, settings.Current.SoundVolume);
            Assert.AreEqual(33, settings.Current.MusicVolume);
            Assert.IsTrue(settings.Current.PushNotificationsEnabled);

            await settings.LoadAsync();
            Assert.AreEqual(55, settings.Current.SoundVolume);
            Assert.AreEqual(33, settings.Current.MusicVolume);
            Assert.IsTrue(settings.Current.PushNotificationsEnabled);
        }

        [Test]
        public async Task RequestClose_CompletesWaitForClose()
        {
            var container = CreateContainer();
            var viewModel = CreateViewModel(container);
            await viewModel.InitializeAsync(EmptyPopupRequest.Instance, CancellationToken.None);

            var waitTask = viewModel.WaitForCloseAsync(CancellationToken.None);
            viewModel.RequestClose();

            await waitTask;
        }

        private static SettingsPopupViewModel CreateViewModel(IObjectResolver container)
        {
            return container.Resolve<SettingsPopupViewModel>();
        }

        private IObjectResolver CreateContainer()
        {
            var builder = new ContainerBuilder();
            PersistenceRegistration.RegisterWithRoot(builder, _tempRoot);
            SettingsRegistration.Register(builder);
            return builder.Build();
        }
    }
}
