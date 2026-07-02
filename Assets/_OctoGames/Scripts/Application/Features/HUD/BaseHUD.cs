using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.ClassicConfirmPopup;
using ClassicConfirmPopupView = OctoGames.App.Features.Popups.ClassicConfirmPopup.ClassicConfirmPopup;
using OctoGames.Popups;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoGames.App.Features.HUD
{
    public sealed class BaseHUD : MonoBehaviour
    {
        [SerializeField] private Text _activeCountText;
        [SerializeField] private Button _addEnemyButton;
        [SerializeField] private Button _addInteractableButton;
        [SerializeField] private Button _addStoryActorButton;
        [SerializeField] private Button _restartButton;

        private IGameplayEntityTracker _tracker;
        private IGameplaySceneStateService _sceneState;
        private IPopupService _popupService;

        [Inject]
        private void Construct(
            IGameplayEntityTracker tracker,
            IGameplaySceneStateService sceneState,
            IPopupService popupService)
        {
            _tracker = tracker;
            _sceneState = sceneState;
            _popupService = popupService;
        }

        private void OnEnable()
        {
            if (_popupService != null)
                _popupService.AllClosed += RefreshHud;

            BindButtons();
            RefreshHud();
        }

        private void OnDisable()
        {
            if (_popupService != null)
                _popupService.AllClosed -= RefreshHud;

            UnbindButtons();
        }

        private void BindButtons()
        {
            if (_addEnemyButton != null)
                _addEnemyButton.onClick.AddListener(OnAddEnemyClicked);

            if (_addInteractableButton != null)
                _addInteractableButton.onClick.AddListener(OnAddInteractableClicked);

            if (_addStoryActorButton != null)
                _addStoryActorButton.onClick.AddListener(OnAddStoryActorClicked);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void UnbindButtons()
        {
            if (_addEnemyButton != null)
                _addEnemyButton.onClick.RemoveListener(OnAddEnemyClicked);

            if (_addInteractableButton != null)
                _addInteractableButton.onClick.RemoveListener(OnAddInteractableClicked);

            if (_addStoryActorButton != null)
                _addStoryActorButton.onClick.RemoveListener(OnAddStoryActorClicked);

            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        private void OnAddEnemyClicked() =>
            AddEntityAsync(GameplayEntityType.Enemy, destroyCancellationToken).Forget();

        private void OnAddInteractableClicked() =>
            AddEntityAsync(GameplayEntityType.Interactable, destroyCancellationToken).Forget();

        private void OnAddStoryActorClicked() =>
            AddEntityAsync(GameplayEntityType.StoryActor, destroyCancellationToken).Forget();

        private void OnRestartClicked() =>
            RestartAsync(destroyCancellationToken).Forget();

        private async UniTask AddEntityAsync(GameplayEntityType type, CancellationToken ct)
        {
            await _sceneState.AddEntityAsync(type, ct);
            RefreshHud();
        }

        private async UniTask RestartAsync(CancellationToken ct)
        {
            var request = new ConfirmPopupRequest(
                "Restart scene?",
                "Current save will be deleted and the initial layout restored.",
                "Restart",
                "Cancel",
                onConfirm: async token =>
                {
                    await _sceneState.ResetToInitialAsync(token);
                    RefreshHud();
                });

            await _popupService.ShowAsync<ClassicConfirmPopupView, ConfirmPopupRequest>(request, ct: ct);
            RefreshHud();
        }

        private void RefreshHud()
        {
            if (_activeCountText == null || _tracker == null)
                return;

            _activeCountText.text = $"Active: {_tracker.GetActiveEntitiesCount<IGameplayEntity>()}";
        }
    }
}
