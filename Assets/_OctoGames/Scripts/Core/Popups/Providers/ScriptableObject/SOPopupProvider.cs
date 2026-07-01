using System;
using System.Collections.Generic;
using UnityEngine;

namespace OctoGames.Popups
{
    [CreateAssetMenu(
        fileName = "Popups Catalog",
        menuName = "OctoGames/Popups Catalog")]
    public sealed class SOPopupProvider : ScriptableObject, IPopupProvider
    {
        [SerializeField] private List<SOPopupProviderItem> _items = new();

        private Dictionary<Type, SOPopupProviderItem> _lookup;

        private void OnEnable() => EnsureLookup();

        public GameObject GetPrefab<TPopup>()
            where TPopup : PopupBaseView
            => GetItemOrThrow(typeof(TPopup)).Prefab;

        public PopupShowPolicy GetShowPolicy(Type popupType)
            => GetItemOrThrow(popupType).DefaultShowPolicy;

        private SOPopupProviderItem GetItemOrThrow(Type popupType)
        {
            if (TryGetItemByType(popupType, out var item))
                return item;

            throw new KeyNotFoundException(
                $"Popup type '{popupType.FullName}' not found in provider '{name}'.");
        }

        private bool TryGetItemByType(Type popupType, out SOPopupProviderItem item)
        {
            item = null;

            if (popupType == null)
                return false;

            EnsureLookup();
            return _lookup.TryGetValue(popupType, out item);
        }

        private void EnsureLookup()
        {
            if (_lookup != null)
                return;

            RebuildLookup();
        }

        private void RebuildLookup()
        {
            _lookup = new Dictionary<Type, SOPopupProviderItem>();

            foreach (var item in _items)
            {
                if (item?.Prefab == null)
                    continue;

                var view = item.Prefab.GetComponent<PopupBaseView>();
                if (view == null)
                    continue;

                _lookup.TryAdd(view.PopupType, item);
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => ValidateAndRebuild();

        private void ValidateAndRebuild()
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var item in _items)
            {
                if (item == null)
                    continue;

                if (item.Prefab == null)
                {
                    Debug.LogError($"Popup provider item in '{name}' has no prefab assigned.", this);
                    continue;
                }

                var view = item.Prefab.GetComponent<PopupBaseView>();
                if (view == null)
                {
                    Debug.LogError(
                        $"Prefab '{item.Prefab.name}' in provider '{name}' is missing {nameof(PopupBaseView)} component.",
                        item.Prefab);
                    continue;
                }

                var typeName = view.PopupType.FullName;

                if (!seen.Add(typeName))
                {
                    Debug.LogError(
                        $"Duplicate popup type '{typeName}' in provider '{name}'.",
                        this);
                }
            }

            _lookup = null;
            RebuildLookup();
        }
#endif
    }
}
