using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Popups;
using OctoGames.App.Features.Settings;
using R3;
using VContainer;

namespace OctoGames.App.Features.Popups.SettingsPopup
{
    public sealed class SettingsPopupViewModel : PopupViewModelBase
    {
        private ISettingsService _settings;

        public ReactiveProperty<int> SoundVolume { get; } = new();
        public ReactiveProperty<int> MusicVolume { get; } = new();
        public ReactiveProperty<bool> PushNotificationsEnabled { get; } = new();

        [Inject]
        private void Construct(ISettingsService settings)
        {
            _settings = settings;
        }

        protected override async UniTask OnInitializeAsync<TRequest>(TRequest request, CancellationToken ct)
        {
            await _settings.LoadAsync(ct);

            var current = _settings.Current;
            SoundVolume.Value = current.SoundVolume;
            MusicVolume.Value = current.MusicVolume;
            PushNotificationsEnabled.Value = current.PushNotificationsEnabled;
        }

        public override async UniTask OnCloseAsync(CancellationToken ct)
        {
            var current = _settings.Current;
            current.SoundVolume = SoundVolume.Value;
            current.MusicVolume = MusicVolume.Value;
            current.PushNotificationsEnabled = PushNotificationsEnabled.Value;

            await _settings.SaveAsync(ct);
            SettingsAudio.Apply(current);
        }

        protected override void OnDispose()
        {
            SoundVolume.Dispose();
            MusicVolume.Dispose();
            PushNotificationsEnabled.Dispose();
        }
    }
}
