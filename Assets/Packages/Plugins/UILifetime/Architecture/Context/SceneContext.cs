using Cr7Sund.IocContainer;
using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public class SceneContext : RouteContext, INodeContext
    {
        private IAssetContainer _uniqueInstanceContainer;
        private IAssetContainer _instanceContainer;
        protected sealed override string Channel
        {
            get => ServerBindDefine.GraphLogger;
        }

        public SceneContext(string sceneName) : base(sceneName)
        {
        }

        public async override PromiseTask AddComponents(INode node, IRouteArgs fragmentContext)
        {
            await base.AddComponents(node, fragmentContext);
            _instanceContainer = ContainerFactory.Create(_sceneName);
            _uniqueInstanceContainer = ContainerFactory.Create(_sceneName);
            await node.AddLifecycle(_instanceContainer, fragmentContext);
            await node.AddLifecycle(_uniqueInstanceContainer, fragmentContext);

            BindInstance<IInstancesContainer>(_instanceContainer);
            BindInstance<IUniqueInstanceContainer>(_uniqueInstanceContainer);
        }

        public override void RemoveComponents()
        {
            base.RemoveComponents();

            Unbind<IInstancesContainer>(_instanceContainer);
            Unbind<IUniqueInstanceContainer>(_uniqueInstanceContainer);
        }
    }
}
