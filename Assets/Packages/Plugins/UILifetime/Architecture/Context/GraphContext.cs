using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public class GraphContext : RouteContext, INodeContext
    {
        IAssetContainer _uniqueInstanceContainer;
        IAssetContainer _instanceContainer;

        protected sealed override string Channel
        {
            get => ServerBindDefine.GraphLogger;
        }

        public GraphContext(string sceneName) : base(sceneName)
        {
        }

        public async sealed override PromiseTask AddComponents(INode node, IRouteArgs fragmentContext)
        {
            await base.AddComponents(node, fragmentContext);

            _instanceContainer = ContainerFactory.Create(_sceneName);
            _uniqueInstanceContainer = ContainerFactory.Create(_sceneName);
            await node.AddLifecycle(_instanceContainer, fragmentContext);
            await node.AddLifecycle(_uniqueInstanceContainer, fragmentContext);

            this.BindAsCrossAndSingleton<ILoadModule, LoadModule>();
            BindInstance<IInstancesContainer>(_instanceContainer);
            BindInstance<IUniqueInstanceContainer>(_uniqueInstanceContainer);
        }

        public sealed override void RemoveComponents()
        {
            base.RemoveComponents();

            Unbind<ILoadModule>();
            Unbind<IInstancesContainer>(_instanceContainer);
            Unbind<IUniqueInstanceContainer>(_uniqueInstanceContainer);
        }
    }

}
