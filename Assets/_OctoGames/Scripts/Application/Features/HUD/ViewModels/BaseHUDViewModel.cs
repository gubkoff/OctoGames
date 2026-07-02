using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.ConfirmPopup;
using ConfirmPopupView = OctoGames.App.Features.Popups.ConfirmPopup.ConfirmPopup;
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
        private IPopupService _popupService;

        [Inject]
        private void Construct(
            IRepository<IGameplayEntity> repository,
            IGameplayEntityService entityService,
            IPopupService popupService)
        {
            _repository = repository;
            _entityService = entityService;
            _popupService = popupService;
        }

        public void Activate()
        {
            _popupService.AllClosed += Refresh;
            Refresh();
        }

        public void Deactivate()
        {
            _popupService.AllClosed -= Refresh;
        }

        public async UniTask AddEntityAsync(GameplayEntityType type, CancellationToken ct)
        {
            await _entityService.AddEntityAsync(type, ct);
            Refresh();
        }

        public async UniTask RestartAsync(CancellationToken ct)
        {
            var request = new ConfirmPopupRequest(
                "Restart scene?",
                "Current save will be deleted and the initial layout restored.",
                "Restart",
                "Cancel",
                onConfirm: async token =>
                {
                    await _entityService.ResetToInitialAsync(token);
                    Refresh();
                });

            await _popupService.ShowAsync<ConfirmPopupView, ConfirmPopupRequest>(request, ct: ct);
            Refresh();
        }

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
