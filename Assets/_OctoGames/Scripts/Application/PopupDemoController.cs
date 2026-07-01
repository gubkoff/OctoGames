using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.Popups;
using OctoGames.App.Features.Popups.SettingsPopup;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App
{
    public sealed class PopupDemoController : MonoBehaviour
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;

        private IPopupService _popupService;

        [Inject]
        private void Construct(IPopupService popupService) => _popupService = popupService;

        private void Awake()
        {
            if (_openButton != null)
                _openButton.onClick.AddListener(OnOpenClicked);

            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnDestroy()
        {
            if (_openButton != null)
                _openButton.onClick.RemoveListener(OnOpenClicked);

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        private void OnOpenClicked()
        {
            OpenPopupAsync(destroyCancellationToken).Forget();
        }

        private void OnCloseClicked()
        {
            ClosePopupAsync(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid OpenPopupAsync(CancellationToken ct)
        {
            await _popupService.ShowAsync<SettingsPopup>(ct: ct);
        }

        private async UniTaskVoid ClosePopupAsync(CancellationToken ct)
        {
            await _popupService.CloseAsync<SettingsPopup>(ct);
        }
    }
}
