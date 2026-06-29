using System;
using UnityEngine;

namespace OctoGames.App.Features.Popups
{
    [Serializable]
    public sealed class SOPopupProviderItem
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private PopupScope _scope = PopupScope.Scene;
        [SerializeField] private PopupShowPolicy _defaultShowPolicy = PopupShowPolicy.Queue;

        public GameObject Prefab => _prefab;
        public PopupScope Scope => _scope;
        public PopupShowPolicy DefaultShowPolicy => _defaultShowPolicy;
    }
}
