using CodeBase.Application.Common.UI;
using CodeBase.Application.Factory;
using CodeBase.Application.Grid.Interaction;
using CodeBase.Core.Factory.Grid;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Application.Scopes
{
    public class BattleScope : LifetimeScope
    {
        [SerializeField] private UIConfig _uiConfig;
        [SerializeField] private FactoryAssets _factoryAssets;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.UseEntryPoints(entryPoints =>
            {
                entryPoints.Add<FactoryPresenter>();
                entryPoints.Add<FactoryUI>();
            });
            
            builder.RegisterComponentInHierarchy<Camera>();
            builder.Register<GridMap>(Lifetime.Singleton);
            builder.Register<UIFactory>(Lifetime.Singleton);
            builder.RegisterComponent(_uiConfig);
            builder.RegisterComponent(_factoryAssets);
            
            builder.RegisterComponentOnNewGameObject<Building>(Lifetime.Singleton, "BuildingSystem");
            builder.RegisterComponentInNewPrefab(_uiConfig.HUD, Lifetime.Singleton);
        }
    }
}