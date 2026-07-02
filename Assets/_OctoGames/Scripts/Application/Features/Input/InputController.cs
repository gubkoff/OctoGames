using System.Threading;
using Cysharp.Threading.Tasks;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.EntityDetailPopup;
using EntityDetailPopupView = OctoGames.App.Features.Popups.EntityDetailPopup.EntityDetailPopup;
using OctoGames.Popups;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace OctoGames.App.Features.Input
{
    public sealed class InputController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _maxDistance = 200f;
        [SerializeField] private LayerMask _layerMask = ~0;

        private IPopupService _popupService;

        [Inject]
        private void Construct(IPopupService popupService) => _popupService = popupService;

        private void Awake()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            if (IsPointerOverUi())
                return;

            if (_camera == null)
                return;

            var screenPosition = mouse.position.ReadValue();
            var ray = _camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, _maxDistance, _layerMask))
                return;

            var entity = hit.collider.GetComponentInParent<GameplayEntityBase>();
            if (entity == null)
                return;

            ShowDetailAsync(entity.Id, destroyCancellationToken).Forget();
        }

        private static bool IsPointerOverUi()
        {
            if (EventSystem.current == null)
                return false;

            return EventSystem.current.IsPointerOverGameObject();
        }

        private async UniTaskVoid ShowDetailAsync(System.Guid entityId, CancellationToken ct)
        {
            await _popupService.ShowAsync<EntityDetailPopupView, EntityDetailPopupRequest>(
                new EntityDetailPopupRequest(entityId),
                ct: ct);
        }
    }
}
