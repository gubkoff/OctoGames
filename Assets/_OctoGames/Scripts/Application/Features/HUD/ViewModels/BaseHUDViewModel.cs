using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.ConfirmPopup;
using ConfirmPopupView = OctoGames.App.Features.Popups.ConfirmPopup.ConfirmPopup;
using OctoGames.App.Features.Popups.SettingsPopup;
using SettingsPopupView = OctoGames.App.Features.Popups.SettingsPopup.SettingsPopup;
using OctoGames.Popups;
using OctoGames.Repository;
using R3;
using VContainer;

namespace OctoGames.App.Features.HUD
{
    public sealed class BaseHUDViewModel : IDisposable
    {
        public ReactiveProperty<int> ActiveCount { get; } = new();

        private IRepository<IGameplayEntity> _repository;
        private IGameplayEntityService _entityService;
        private IGameplayEntityStateService _entityStateService;
        private IPopupService _popupService;

        [Inject]
        private void Construct(
            IRepository<IGameplayEntity> repository,
            IGameplayEntityService entityService,
            IGameplayEntityStateService entityStateService,
            IPopupService popupService)
        {
            _repository = repository;
            _entityService = entityService;
            _entityStateService = entityStateService;
            _popupService = popupService;
        }

        public void Activate()
        {
            _repository.Changed += Refresh;
            _entityStateService.Changed += Refresh;
            Refresh();
        }

        public void Deactivate()
        {
            _repository.Changed -= Refresh;
            _entityStateService.Changed -= Refresh;
        }

        public UniTask AddEntityAsync(GameplayEntityType type, CancellationToken ct) =>
            _entityService.AddEntityAsync(type, ct);

        public async UniTask RestartAsync(CancellationToken ct)
        {
            var request = new ConfirmPopupRequest(
                "Restart scene?",
                "Current save will be deleted and the initial layout restored.",
                "Restart",
                "Cancel",
                onConfirm: token => _entityService.ResetToInitialAsync(token));

            await _popupService.ShowAsync<ConfirmPopupView, ConfirmPopupRequest>(request, ct: ct);
        }

        public UniTask OpenSettingsAsync(CancellationToken ct) =>
            _popupService.ShowAsync<SettingsPopupView>(ct: ct);

        public void Refresh()
        {
            ActiveCount.Value = GetActiveEntitiesCount();
        }

        private int GetActiveEntitiesCount()
        {
            var all = _repository.GetAll();
            var activeCount = 0;

            for (var i = 0; i < all.Count; i++)
            {
                if (all[i].IsActive)
                    activeCount++;
            }

            return activeCount;
        }

        public void Dispose()
        {
            Deactivate();
            ActiveCount.Dispose();
        }
    }
}
