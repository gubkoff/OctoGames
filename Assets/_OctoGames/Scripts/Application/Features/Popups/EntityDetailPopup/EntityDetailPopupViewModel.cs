using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.ClassicConfirmPopup;
using ClassicConfirmPopupView = OctoGames.App.Features.Popups.ClassicConfirmPopup.ClassicConfirmPopup;
using OctoGames.Popups;
using OctoGames.Repository;
using R3;
using VContainer;

namespace OctoGames.App.Features.Popups.EntityDetailPopup
{
    public sealed class EntityDetailPopupViewModel : PopupViewModelBase
    {
        public ReactiveProperty<string> Title { get; } = new();
        public ReactiveProperty<string> TypeLabel { get; } = new();
        public ReactiveProperty<string> StateLabel { get; } = new();
        public ReactiveProperty<bool> ShowDisable { get; } = new();
        public ReactiveProperty<bool> ShowEnable { get; } = new();
        public ReactiveProperty<bool> ShowComplete { get; } = new();
        public ReactiveProperty<bool> ShowDelete { get; } = new(true);

        private IRepository<IGameplayEntity> _repository;
        private IGameplaySceneStateService _sceneState;
        private IPopupService _popupService;
        private Guid _entityId;
        private bool _pendingDelete;

        [Inject]
        private void Construct(
            IRepository<IGameplayEntity> repository,
            IGameplaySceneStateService sceneState,
            IPopupService popupService)
        {
            _repository = repository;
            _sceneState = sceneState;
            _popupService = popupService;
        }

        protected override UniTask OnInitializeAsync<TRequest>(TRequest request, CancellationToken ct)
        {
            var detailRequest = (EntityDetailPopupRequest)(object)request;
            _entityId = detailRequest.EntityId;
            RefreshFromEntity();
            return UniTask.CompletedTask;
        }

        public async UniTask DisableAsync(CancellationToken ct)
        {
            await ApplyStateAsync(GameplayEntityState.Disabled, ct);
        }

        public async UniTask EnableAsync(CancellationToken ct)
        {
            await ApplyStateAsync(GameplayEntityState.Active, ct);
        }

        public async UniTask CompleteAsync(CancellationToken ct)
        {
            await ApplyStateAsync(GameplayEntityState.Completed, ct);
        }

        public void RequestDelete()
        {
            _pendingDelete = true;
            RequestClose();
        }

        public override async UniTask OnCloseAsync(CancellationToken ct)
        {
            if (!_pendingDelete)
                return;

            _pendingDelete = false;
            await ShowDeleteConfirmAsync(ct);
        }

        private async UniTask ShowDeleteConfirmAsync(CancellationToken ct)
        {
            if (!_repository.TryGet(_entityId, out _))
                return;

            var entityId = _entityId;
            var request = new ConfirmPopupRequest(
                "Delete entity?",
                "This action cannot be undone.",
                "Delete",
                "Cancel",
                onConfirm: async token =>
                {
                    _sceneState.DestroyEntity(entityId);
                    await _sceneState.SaveAsync(token);
                });

            await _popupService.ShowAsync<ClassicConfirmPopupView, ConfirmPopupRequest>(request, ct: ct);
        }

        private async UniTask ApplyStateAsync(GameplayEntityState state, CancellationToken ct)
        {
            if (!_repository.TryGet(_entityId, out _))
                return;

            _sceneState.SetEntityState(_entityId, state);
            await _sceneState.SaveAsync(ct);
            RefreshFromEntity();
        }

        private void RefreshFromEntity()
        {
            if (!_repository.TryGet(_entityId, out var entity))
            {
                Title.Value = "Entity not found";
                TypeLabel.Value = string.Empty;
                StateLabel.Value = string.Empty;
                ShowDisable.Value = false;
                ShowEnable.Value = false;
                ShowComplete.Value = false;
                ShowDelete.Value = false;
                return;
            }

            Title.Value = $"Entity {entity.Id.ToString()[..8]}";
            TypeLabel.Value = entity.Data.Type.ToString();
            StateLabel.Value = entity.Data.State.ToString();
            ShowDisable.Value = GameplayEntityTransitions.CanDisable(entity);
            ShowEnable.Value = GameplayEntityTransitions.CanEnable(entity);
            ShowComplete.Value = GameplayEntityTransitions.CanComplete(entity);
            ShowDelete.Value = true;
        }

        protected override void OnDispose()
        {
            Title.Dispose();
            TypeLabel.Dispose();
            StateLabel.Dispose();
            ShowDisable.Dispose();
            ShowEnable.Dispose();
            ShowComplete.Dispose();
            ShowDelete.Dispose();
        }
    }
}
