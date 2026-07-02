using OctoGames.App;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.HUD;
using OctoGames.App.Features.Popups.ConfirmPopup;
using OctoGames.App.Features.Popups.EntityDetailPopup;
using OctoGames.Repository;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoGames.Composition
{
    public static class GameplayRegistration
    {
        public static void Register(
            IContainerBuilder builder,
            SOGameplayEntitiesInitialState initialState,
            SOGameplayEntityCatalog entityCatalog,
            Transform entitiesRoot)
        {
            if (initialState == null)
                throw new System.InvalidOperationException(
                    "SOGameplayEntitiesInitialState is not assigned on GameLifetimeScope.");

            if (entityCatalog == null)
                throw new System.InvalidOperationException(
                    "SOGameplayEntityCatalog is not assigned on GameLifetimeScope.");

            if (entitiesRoot == null)
                throw new System.InvalidOperationException(
                    "Entities root Transform is not assigned on GameLifetimeScope.");

            builder.Register<GameplayEntityRepository>(Lifetime.Singleton)
                .As<IRepository<IGameplayEntity>>();

            builder.RegisterInstance(initialState);
            builder.RegisterInstance(entityCatalog);
            builder.RegisterInstance(entitiesRoot);

            builder.Register<GameplayEntityStateService>(Lifetime.Singleton)
                .As<IGameplayEntityStateService>();

            builder.Register<GameplayEntitySpawner>(Lifetime.Singleton)
                .As<IGameplayEntitySpawner>();

            builder.Register<GameplayEntityService>(Lifetime.Singleton)
                .As<IGameplayEntityService>();

            builder.Register<ConfirmPopupViewModel>(Lifetime.Transient);
            builder.Register<EntityDetailPopupViewModel>(Lifetime.Transient);
            builder.Register<BaseHUDViewModel>(Lifetime.Transient);

            builder.RegisterEntryPoint<ApplicationEntryPoint>();
        }
    }
}
