using Cr7Sund.IocContainer;
namespace Cr7Sund.LifeTime
{
    public class RouteContext : CrossContext
    {
        protected virtual string Channel { get; }
        protected readonly string _sceneName;
        
        public RouteContext(string sceneName)
        {
            _sceneName = sceneName;
        }


        public virtual PromiseTask AddComponents(INode node, IRouteArgs fragmentContext)
        {
            var log = InternalLoggerFactory.Create(node.Key.Key);
            BindInstanceAsCrossContext<IInternalLog>(log, Channel);
            OnMappedBindings();

            return PromiseTask.CompletedTask;
        }

        public virtual void RemoveComponents()
        {
            Unbind<IInternalLog>(Channel);
            OnUnMappedBindings();
        }
        
        protected virtual void OnMappedBindings(){}
        protected virtual void OnUnMappedBindings(){}
    }
}
