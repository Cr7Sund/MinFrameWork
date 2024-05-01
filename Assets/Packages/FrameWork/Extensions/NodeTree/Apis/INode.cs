using Cr7Sund.Package.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface INode : ILifeTime, ILoadAsync, IInjectable, IRunnable, IInitialize, IDestroy
    {
        IContext Context { get; }
        INode Parent { get; set; }
        IPromise AddStatus { get; set; }
        IPromise RemoveStatus { get; set; }
        NodeState NodeState { get; }
        IAssetKey Key { get; }
        int ChildCount { get; }
        
        PromiseTask PreLoadChild(INode child);
        PromiseTask AddChildAsync(INode child, bool overwrite = false);
        PromiseTask UnloadChildAsync(INode child, bool overwrite = false);
        PromiseTask RemoveChildAsync(INode child, bool overwrite = false);
        INode GetChild(int index);


        #region Load
        PromiseTask LoadAsync();
        PromiseTask PreLoadAsync();
        PromiseTask UnloadAsync();
        void CancelLoad();
        void CancelUnload();

        void SetAdding();
        void StartPreload();
        void SetReady();
        void StartUnload(bool shouldUnload);
        void EndUnLoad(bool unload);
        void CancelCurTask();
        #endregion

        #region LifeCycle
        PromiseTask SetActive(bool active);
        PromiseTask OnStart();
        PromiseTask OnEnable();
        PromiseTask OnDisable();
        PromiseTask OnStop();
        #endregion

    }
}
