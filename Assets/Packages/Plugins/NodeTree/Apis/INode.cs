using System.Threading;
using Cr7Sund.IocContainer;
using Cr7Sund.Package.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface INode : ILifeTime, ILoadAsync, IInjectable, IRunnable, IInitialize
    {
        INodeContext Context { get; }
        INode Parent { get; set; }
        IPromise AddStatus { get; set; }
        IPromise RemoveStatus { get; set; }
        NodeState NodeState { get; }
        IAssetKey Key { get; }
        int ChildCount { get; }
        UnsafeCancellationTokenSource AddCancellation { get; }
        UnsafeCancellationTokenSource RemoveCancellation { get; }

        PromiseTask PreLoadChild(INode child);
        PromiseTask AddChildAsync(INode child, bool overwrite = false);
        PromiseTask UnloadChildAsync(INode child, bool overwrite = false);
        PromiseTask RemoveChildAsync(INode child, bool overwrite = false);
        void CancelCurTask();
        void Destroy(INodeContext parentContext);
        INode GetChild(int index);

        #region Load
        PromiseTask LoadAsync(UnsafeCancellationToken cancellation);
        PromiseTask PreLoadAsync(UnsafeCancellationToken cancellation);
        PromiseTask UnloadAsync(UnsafeCancellationToken cancellation);
        void CancelLoad();
        void CancelUnLoad();
        void SetAdding();
        void StartPreload();
        void SetReady();
        void StartUnload(bool shouldUnload);
        void EndUnLoad(bool unload);
        #endregion

        #region LifeCycle
        PromiseTask SetActive(bool active);
        PromiseTask OnStart(UnsafeCancellationToken cancellation);
        PromiseTask OnEnable();
        PromiseTask OnDisable(bool closeImmediately);
        PromiseTask OnStop();
        void OnCancelLoad();
        void OnCancelUnLoad();
        #endregion

    }
}
