using System;
using UnityEngine;

namespace OctoGames.App.Features.Settings
{
    public static class SettingsAudio
    {
        public static void Apply(GameSettings settings)
        {
            var sound = settings.SoundVolume / (float)GameSettings.MaxVolume;
            var music = settings.MusicVolume / (float)GameSettings.MaxVolume;
            AudioListener.volume = Math.Clamp(sound * music, 0f, 1f);
        }
    }
}
