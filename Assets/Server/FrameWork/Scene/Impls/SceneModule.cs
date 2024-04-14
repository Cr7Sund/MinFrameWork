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


        public async PromiseTask SwitchScene(IAssetKey key)
        {
            await SwitchNode(key);
        }
        public async PromiseTask RemoveScene(IAssetKey key)
        {
            await RemoveNode(key);
        }
        public async PromiseTask UnloadScene(IAssetKey key)
        {
            await UnloadNode(key);
        }
        public async PromiseTask PreLoadScene(IAssetKey key)
        {
            await PreLoadNode(key);
        }
        public async PromiseTask AddScene(IAssetKey key)
        {
            await AddNode(key);
        }

        protected override INode CreateNode(IAssetKey key)
        {
            return SceneCreator.Create((SceneKey)key);
        }

        #region  Event
        protected override void DispatchSwitch(IAssetKey curScene, IAssetKey lastScene)
        {
            var e = _eventBus.CreateEvent<SwitchSceneEvent>();
            e.LastScene = lastScene;
            e.CurScene = curScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddBegin(IAssetKey targetScene)
        {
            var e = _eventBus.CreateEvent<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchAddEnd(IAssetKey targetScene)
        {
            var e = _eventBus.CreateEvent<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveBegin(IAssetKey targetScene)
        {
            var e = _eventBus.CreateEvent<AddSceneBeginEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }
        protected override void DispatchRemoveEnd(IAssetKey targetScene)
        {
            var e = _eventBus.CreateEvent<AddSceneEndEvent>();
            e.TargetScene = targetScene;
            _eventBus.Dispatch(e);
        }

        #endregion

    }
}
