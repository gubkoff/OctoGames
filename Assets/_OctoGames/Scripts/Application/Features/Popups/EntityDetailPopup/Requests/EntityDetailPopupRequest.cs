using System;
using OctoGames.Popups;

namespace OctoGames.App.Features.Popups.EntityDetailPopup
{
    public sealed class EntityDetailPopupRequest : IPopupRequest
    {
        public EntityDetailPopupRequest(Guid entityId) => EntityId = entityId;

        public Guid EntityId { get; }
    }
}
