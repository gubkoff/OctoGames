using OctoGames.App.Features.Entities;
using OctoGames.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoGames.Composition
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private SOPopupProvider _popupProvider;
        [SerializeField] private PopupRoot _popupRoot;
        [SerializeField] private SOGameplayEntitiesInitialState _initialState;
        [SerializeField] private SOGameplayEntityCatalog _entityCatalog;
        [SerializeField] private Transform _entitiesRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            PersistenceRegistration.Register(builder);
            SettingsRegistration.Register(builder);
            PopupRegistration.Register(builder, _popupProvider, _popupRoot);
            GameplayRegistration.Register(builder, _initialState, _entityCatalog, _entitiesRoot);
        }
    }
}
