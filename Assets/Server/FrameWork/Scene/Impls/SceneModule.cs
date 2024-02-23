using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Apis;

namespace Cr7Sund.Server.Scene.Impl
{
    public class SceneModule : LoadModule, ISceneModule
    {
        [Inject]
        private IGameNode _gameNode;
        public INode FocusScene => _focusNode;

        protected override INode _parentNode
        {
            get
            {
                return _gameNode;
            }
        }


        public IPromise<INode> SwitchScene(IAssetKey key)
        {
            return SwitchNode(key);
        }
        public IPromise<INode> RemoveScene(IAssetKey key)
        {
            return RemoveNode(key);
        }
        public IPromise<INode> UnloadScene(IAssetKey key)
        {
            return UnloadNode(key);
        }
        public IPromise<INode> PreLoadScene(IAssetKey key)
        {
            return PreLoadNode(key);
        }
        public IPromise<INode> AddScene(IAssetKey key)
        {
            return AddNode(key);
        }

        protected override INode CreateNode(IAssetKey key)
        {
            return SceneCreator.Create((SceneKey)key);
        }

        protected override IPromise<INode> OnAdded(INode node)
        {
            var sceneNode = node as SceneNode;
            return sceneNode.ActiveScene()
                .Then(() => Promise<INode>.Resolved(node));
        }

        #region  Event
        protected override void DispatchSwitch(IAssetKey curScene, IAssetKey lastScene)
        {
            var e = _poolBinder.AutoCreate<SwitchSceneEvent>();
            e.LastScene = lastScene;
            e.CurScene = curScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetScene)
        {
            var e = _poolBinder.AutoCreate<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }

        #endregion

    }
}
