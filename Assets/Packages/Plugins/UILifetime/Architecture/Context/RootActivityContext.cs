using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.IocContainer;
namespace Cr7Sund.LifeTime
{
    public class RootActivityContext : RouteContext, INodeContext
    {
        
        protected sealed override string Channel
        {
            get => ServerBindDefine.GameLogger;
        }
        
        public RootActivityContext() : base(string.Empty)
        {
            _crossContextInjectionBinder.CrossContextBinder = new CrossContextInjectionBinder();
        }

        public async override PromiseTask AddComponents(INode node, IRouteArgs fragmentContext)
        {
            await base.AddComponents(node, fragmentContext);

            var assetLoader = AssetLoaderFactory.CreateLoader();
            var sceneLoader = AssetLoaderFactory.CreateSceneLoader();
            var instanceContainer = ContainerFactory.Create(_sceneName);
            var uniqueInstanceContainer = ContainerFactory.Create(_sceneName);

            BindInstanceAsCrossContext<IAssetLoader>(assetLoader);
            BindInstanceAsCrossContext<ISceneLoader>(sceneLoader);
            BindAsCrossAndSingleton<NavHost, NavHost>();
            BindInstanceAsCrossContext<IInstancesContainer>(instanceContainer, ServerBindDefine.GameInstance);
            BindInstanceAsCrossContext<IUniqueInstanceContainer>(uniqueInstanceContainer, ServerBindDefine.GameUniqueInstance);
            BindAsCrossAndSingleton<TransactionFactory, TransactionFactory>();

        }

        public override void RemoveComponents()
        {
            base.RemoveComponents();

            Unbind<IInstancesContainer>(ServerBindDefine.GameInstance);
            Unbind<IUniqueInstanceContainer>(ServerBindDefine.GameUniqueInstance);
            Unbind<IAssetLoader>();
            Unbind<ISceneLoader>();
            Unbind<NavHost>();
            Unbind<TransactionFactory>();
        }
    }
}
