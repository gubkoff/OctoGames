using System;

namespace OctoGames.App.Features.Settings
{
    public sealed class GameSettings
    {
        public const int MinVolume = 1;
        public const int MaxVolume = 100;
        public const string PersistenceKey = "settings";

        private int _soundVolume = MaxVolume;
        private int _musicVolume = MaxVolume;

        public int SoundVolume
        {
            get => _soundVolume;
            set => _soundVolume = Math.Clamp(value, MinVolume, MaxVolume);
        }

        public int MusicVolume
        {
            get => _musicVolume;
            set => _musicVolume = Math.Clamp(value, MinVolume, MaxVolume);
        }

        public bool PushNotificationsEnabled { get; set; }

        public static GameSettings CreateDefault() => new();
    }
}
