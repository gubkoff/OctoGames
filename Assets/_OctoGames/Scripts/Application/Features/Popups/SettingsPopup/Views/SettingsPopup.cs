using OctoGames.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace OctoGames.App.Features.Popups.SettingsPopup
{
    public sealed class SettingsPopup : PopupBaseView
    {
        [SerializeField] private Slider _soundVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Toggle _pushNotificationsToggle;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _soundVolumeLabel;
        [SerializeField] private Text _musicVolumeLabel;

        internal Slider SoundVolumeSlider => _soundVolumeSlider;
        internal Slider MusicVolumeSlider => _musicVolumeSlider;
        internal Toggle PushNotificationsToggle => _pushNotificationsToggle;
        internal Button CloseButton => _closeButton;
        internal Text SoundVolumeLabel => _soundVolumeLabel;
        internal Text MusicVolumeLabel => _musicVolumeLabel;
    }
}
