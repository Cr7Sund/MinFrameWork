using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;
using UnityEngine;

namespace Cr7Sund.Server.UI.Impl
{
    public class UINode : UpdateNode, IUINode
    {
        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IPoolBinder _poolBinder;
        private IAssetPromise _assetPromise;



        public IUIView View { get; set; }
        public string PageId { get; set; }
        public bool IsTransitioning { get; private set; }
        public IUIController Controller { get; set; }

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var uiNode = content as UINode;
            var uiKey = uiNode.Key as UIKey;

            IPromise preparePromise = Controller.Prepare(uiKey.Intent);
            IPromise loadPromise = null;
            if (Application.isPlaying)
            {
                _assetPromise = uiKey.LoadAsync ? _assetLoader.InstantiateAsync(uiKey)
                                                  : _assetLoader.Instantiate(uiKey);
                loadPromise = _assetPromise.Then(_ => { });

            }
            else
            {
                loadPromise = base.OnLoadAsync(content)
                                  .Then(_ => { });
            }

            return Promise.All(preparePromise, loadPromise)
                .ContinueWith(() => Promise<INode>.Resolved(content));
        }
        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            if (Application.isPlaying)
            {
                AssertUtil.NotNull(_assetPromise, UIExceptionType.no_load_promise);

                _assetLoader.ReleaseInstance(_assetPromise);
            }

            return base.OnUnloadAsync(content);
        }


        public IPromise BeforeExit(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.Show(push);

            if (push)
                return Controller.WillPushExit();
            else
                return Controller.WillPopExit();
        }
        public IPromise BeforeEnter(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.Hide(push);

            if (push)
                return Controller.WillPushEnter();
            else
                return Controller.WillPushEnter();
        }
        public IPromise Enter(bool push, IUINode partnerView, bool playAnimation)
        {
            View.Show(push);
            Controller.Enable();
            if (playAnimation)
                return View.Animate(push);
            else
                return Promise.Resolved();
        }
        public IPromise Exit(bool push, IUINode partnerView, bool playAnimation)
        {
            View.Hide(push);
            Controller.Disable();
            if (playAnimation)
                return View.Animate(push);
            else
                return Promise.Resolved();
        }

        public IPromise AfterEnter(bool push, IUINode exitPage)
        {
            IsTransitioning = false;

            if (push)
                return Controller.DidPushEnter();
            else
                return Controller.DidPopEnter();
        }
        public IPromise AfterExit(bool push, IUINode enterPage)
        {
            IsTransitioning = false;

            if (push)
                return Controller.DidPushExit();
            else
                return Controller.DidPopExit();
        }


        #region  LifeCycle
        public override void Inject()
        {
            if (IsInjected)
                return;

            _context.InjectionBinder.Bind<IUINode>().To(this);
            _context.InjectionBinder.Injector.Inject(Controller);
            base.Inject();
        }

        public override void DeInject()
        {
            if (!IsInjected)
                return;
            IsInjected = false;

            _context.InjectionBinder.Injector.Uninject(this);
            _context.InjectionBinder.Injector.Uninject(Controller);
            _context.InjectionBinder.Unbind<INode>(this);
        }

        protected override void OnInit()
        {
            // duplicate
            // and the controller should not control anything 
            // that has not instantiated
        }
        protected override void OnStart()
        {
            // only call once
            Controller.Start();
        }
        protected override void OnEnable()
        {
            // call after transition
            // and always be called after Start
            // VM.Enable();
        }
        protected override void OnDisable()
        {
            // call after transition
            // and always be called after Start
            // VM.Disable();
        }

        protected override void OnStop()
        {
            // only call once
            Controller.Stop();
        }
        protected override void OnDispose()
        {
            // duplicate
        }

        #endregion
    }
}