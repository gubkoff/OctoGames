using OctoGames.App;
using OctoGames.App.Features.Entities;
using OctoGames.App.Features.Popups.ClassicConfirmPopup;
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
                    "SOGameplayEntitiesInitialState is not assigned on EntityTrackerLifetimeScope.");

            if (entityCatalog == null)
                throw new System.InvalidOperationException(
                    "SOGameplayEntityCatalog is not assigned on EntityTrackerLifetimeScope.");

            if (entitiesRoot == null)
                throw new System.InvalidOperationException(
                    "Entities root Transform is not assigned on EntityTrackerLifetimeScope.");

            builder.Register<GameplayEntityRepository>(Lifetime.Singleton)
                .As<IRepository<IGameplayEntity>>();

            builder.Register<GameplayEntityTracker>(Lifetime.Singleton)
                .As<IGameplayEntityTracker>();

            builder.RegisterInstance(initialState);
            builder.RegisterInstance(entityCatalog);
            builder.RegisterInstance(entitiesRoot);

            builder.Register<GameplaySceneStateService>(Lifetime.Singleton)
                .As<IGameplaySceneStateService>();

            builder.Register<ConfirmPopupViewModel>(Lifetime.Transient);
            builder.Register<EntityDetailPopupViewModel>(Lifetime.Transient);

            builder.RegisterEntryPoint<ApplicationEntryPoint>();
        }
    }
}
