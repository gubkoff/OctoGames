using System;

namespace OctoGames.Gameplay.Features.Settings
{
    public sealed class GameSettings
    {
        public const int MinVolume = 1;
        public const int MaxVolume = 100;
        public const string PersistenceKey = "settings";

        public int SoundVolume { get; set; } = MaxVolume;
        public int MusicVolume { get; set; } = MaxVolume;
        public bool PushNotificationsEnabled { get; set; }

        public static GameSettings CreateDefault() => new();

        public void Normalize()
        {
            SoundVolume = Math.Clamp(SoundVolume, MinVolume, MaxVolume);
            MusicVolume = Math.Clamp(MusicVolume, MinVolume, MaxVolume);
        }
    }
}
