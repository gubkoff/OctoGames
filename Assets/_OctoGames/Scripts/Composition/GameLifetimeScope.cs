using OctoGames.Popups;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace OctoGames.Composition
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private SOPopupProvider _popupProvider;
        [SerializeField] private PopupRoot _popupRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            PersistenceRegistration.Register(builder);
            SettingsRegistration.Register(builder);
            PopupRegistration.Register(builder, _popupProvider, _popupRoot);
        }
    }
}
