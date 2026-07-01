using System;
using UnityEngine;

namespace OctoGames.Popups
{
    public interface IPopupProvider
    {
        GameObject GetPrefab<TPopup>()
            where TPopup : PopupBaseView;

        PopupShowPolicy GetShowPolicy(Type popupType);
    }
}
