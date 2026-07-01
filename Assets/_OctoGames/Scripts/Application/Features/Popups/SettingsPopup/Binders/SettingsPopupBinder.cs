using OctoGames.Popups;
using OctoGames.App.Features.Settings;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.Popups.SettingsPopup
{
    public sealed class SettingsPopupBinder : PopupBinderBase<SettingsPopup>
    {
        public override IPopupViewModel ResolveViewModel(IObjectResolver resolver)
        {
            return resolver.Resolve<SettingsPopupViewModel>();
        }

        protected override void BindView(
            SettingsPopup popup,
            IPopupViewModel viewModel,
            CompositeDisposable disposables)
        {
            var settingsViewModel = (SettingsPopupViewModel)viewModel;

            BindCloseButton(popup.CloseButton, settingsViewModel, disposables);

            BindVolumeSlider(
                popup.SoundVolumeSlider,
                popup.SoundVolumeLabel,
                settingsViewModel.SoundVolume,
                disposables);

            BindVolumeSlider(
                popup.MusicVolumeSlider,
                popup.MusicVolumeLabel,
                settingsViewModel.MusicVolume,
                disposables);

            if (popup.PushNotificationsToggle != null)
            {
                popup.PushNotificationsToggle.isOn = settingsViewModel.PushNotificationsEnabled.Value;
                popup.PushNotificationsToggle.OnValueChangedAsObservable()
                    .Subscribe(value => settingsViewModel.PushNotificationsEnabled.Value = value)
                    .AddTo(disposables);

                settingsViewModel.PushNotificationsEnabled
                    .Subscribe(value => popup.PushNotificationsToggle.SetIsOnWithoutNotify(value))
                    .AddTo(disposables);
            }
        }

        private static void BindVolumeSlider(
            Slider slider,
            Text label,
            ReactiveProperty<int> volume,
            CompositeDisposable disposables)
        {
            if (slider == null)
                return;

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.SetValueWithoutNotify(VolumeToSlider(volume.Value));

            slider.OnValueChangedAsObservable()
                .Subscribe(value => volume.Value = SliderToVolume(value))
                .AddTo(disposables);

            volume
                .Subscribe(value =>
                {
                    slider.SetValueWithoutNotify(VolumeToSlider(value));

                    if (label != null)
                        label.text = value.ToString();
                })
                .AddTo(disposables);
        }

        private static float VolumeToSlider(int volume)
        {
            var range = GameSettings.MaxVolume - GameSettings.MinVolume;
            return range == 0
                ? 0f
                : (volume - GameSettings.MinVolume) / (float)range;
        }

        private static int SliderToVolume(float sliderValue)
        {
            var range = GameSettings.MaxVolume - GameSettings.MinVolume;
            var volume = GameSettings.MinVolume + Mathf.RoundToInt(sliderValue * range);
            return System.Math.Clamp(volume, GameSettings.MinVolume, GameSettings.MaxVolume);
        }
    }
}
