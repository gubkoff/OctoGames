using UnityEngine;

namespace OctoGames.Popups
{
    public sealed class PopupRoot : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        public Transform Container => _container != null ? _container : transform;

        private void Reset()
        {
            _container = transform;
        }
    }
}
