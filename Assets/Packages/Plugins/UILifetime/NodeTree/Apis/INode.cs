using Cr7Sund.Package.Api;
using Cr7Sund.LifeTime;
namespace Cr7Sund.LifeTime
{
    public interface INode : ILifeTime, IRunnable, IInitialize, ILifeCycleOwner
    {
        // INodeContext Context { get; }
        INode Parent { get; set; }
        IPromise AddStatus { get; }
        IPromise RemoveStatus { get; }
        NodeState NodeState { get; }
        IRouteKey Key { get; }
        int ChildCount { get; }
        UnsafeCancellationTokenSource AddCancellation { get; }
        UnsafeCancellationTokenSource RemoveCancellation { get; }

        void Init(IRouteKey assetKey);
        PromiseTask StartCreate(IRouteArgs fragmentContext, INode parentNode);
        PromiseTask PreloadView(IRouteArgs fragmentContext);
        PromiseTask EndLoad();
        PromiseTask AppendTransition();
        void EndCreate();
        void StartDestroy(bool isRemove);
        void EndDestroy(bool isRemoved = false);

        void Destroy();
        INode GetChild(int index);
        void CancelAddNode();
        void CancelUnLoadNode();

        void AddChild(INode child);
        void RemoveChild(INode child);
    }
}
