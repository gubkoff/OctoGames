using System;
using UnityEngine;

namespace OctoGames.App.Features.Popups
{
    public interface IPopupProvider
    {
        GameObject GetPrefab<TPopup>()
            where TPopup : PopupBaseView;

        PopupShowPolicy GetShowPolicy(Type popupType);
    }
}
