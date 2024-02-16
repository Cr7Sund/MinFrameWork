﻿using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.Server.Apis;

namespace Cr7Sund.Server.Impl
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

        protected override INode CreateNode(IAssetKey key)
        {
            return SceneCreator.Create((SceneKey)key);
        }
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
    }
}
