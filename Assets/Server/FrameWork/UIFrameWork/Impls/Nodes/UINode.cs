using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using UnityEngine;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.UI.Impl
{
    public class UINode : UpdateNode, IUINode
    {
        [Inject] private IAssetLoader _assetLoader;

        private IAssetPromise _assetPromise;
        // Unload
        // GetInstance
        // Preload
        // private IPanelContainer _panelContainer;

        public IUIView View { get; private set; }
        public string PageId { get; set; }
        public bool IsTransitioning { get; private set; }
        public IUIController Controller { get; private set; }


        private UINode(IAssetKey assetKey) : base(assetKey)
        {

        }
        public UINode(IAssetKey assetKey, IUIView uiView, IUIController uiController) : this(assetKey)
        {
            View = uiView;
            Controller = uiController;
            PageId = System.Guid.NewGuid().ToString();
        }

        public IPromise BeforeExit(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.BeforeExit();

            if (push)
                return Controller.WillPushExit();
            else
                return Controller.WillPopExit();
        }

        public IPromise BeforeEnter(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.BeforeEnter();

            if (push)
                return Controller.WillPushEnter();
            else
                return Controller.WillPushEnter();
        }

        public IPromise Enter(bool push, IUINode partnerView, bool playAnimation)
        {
            Controller.Enable();

            return View.EnterRoutine(push, partnerView, playAnimation);
        }

        public IPromise Exit(bool push, IUINode partnerView, bool playAnimation)
        {
            Controller.Disable();

            return View.ExitRoutine(push, partnerView, playAnimation);
        }

        public IPromise AfterEnter(bool push, IUINode exitPage)
        {
            IsTransitioning = false;
            View.AfterEnter();

            if (push)
                return Controller.DidPushEnter();
            else
                return Controller.DidPopEnter();
        }

        public IPromise AfterExit(bool push, IUINode enterPage)
        {
            IsTransitioning = false;
            View.AfterExit();

            if (push)
                return Controller.DidPushExit();
            else
                return Controller.DidPopExit();
        }

        protected override IPromise<INode> OnPreloadAsync(INode content)
        {
            var uiKey = content.Key as UIKey;
            IPromise preparePromise = null;
            IPromise loadPromise = null;

            preparePromise = Controller.Prepare(uiKey.Intent);
            if (Application.isPlaying)
            {
                _assetPromise = _assetLoader.LoadAsync<Object>(content.Key);
                loadPromise = _assetPromise.Then(_ => { });
            }
            else
            {
                loadPromise = base.OnPreloadAsync(content)
                                  .Then(_ => { });
            }

            return Promise.All(preparePromise, loadPromise)
                    .Then(() => Promise<INode>.Resolved(content));
        }

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var uiNode = content as UINode;
            var uiKey = uiNode.Key as UIKey;

            IPromise preparePromise = null;
            IPromise loadPromise = null;

            preparePromise = Controller.Prepare(uiKey.Intent);
            if (Application.isPlaying)
            {
                AssertUtil.IsNull(_assetPromise, UIExceptionType.empty_page_animSource);

                _assetPromise = uiKey.LoadAsync ? _assetLoader.LoadAsync<Object>(uiKey)
                                                  : _assetLoader.Load<Object>(uiKey);
                loadPromise = _assetPromise.Then(_ => { });
            }
            else
            {
                loadPromise = base.OnLoadAsync(content)
                                  .Then(_ => { });
            }

            return Promise.All(preparePromise, loadPromise)
                .Then(() => Promise<INode>.Resolved(content));
        }

        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            if (Application.isPlaying)
            {
                _assetLoader.Unload<Object>(_assetPromise);
            }

            return base.OnUnloadAsync(content);
        }

        #region  LifeCycle

        protected override void OnInject()
        {
            base.OnInject();
            _context.InjectionBinder.Injector.Inject(Controller);
            _context.InjectionBinder.Injector.Inject(View);
        }

        protected override void OnDeject()
        {
            base.OnDeject();
            _context.InjectionBinder.Injector.Uninject(View);
            _context.InjectionBinder.Injector.Uninject(Controller);
        }

        protected override void OnInit()
        {
            // duplicate
            // and the controller should not control anything 
            // that has not instantiated
        }

        protected override void OnStart()
        {
            if (Application.isPlaying)
            {
                View.Start(_assetPromise.GetResult<Object>(), Parent);
            }

            // only call once
            Controller.Start();
        }

        protected override void OnEnable()
        {
            // call after transition
            // and always be called after Start
            // VM.Enable();

            if (Application.isPlaying)
            {
                View.Enable(Parent);
            }
        }

        protected override void OnUpdate(int milliseconds)
        {
            base.OnUpdate(milliseconds);
            View.Update(milliseconds);
        }

        protected override void OnDisable()
        {

            View.Disable();
        }

        protected override void OnStop()
        {
            // only call once
            Controller.Stop();
            View.Stop();
        }

        protected override void OnDispose()
        {
            // duplicate
            View.Dispose();
        }

        #endregion
    }
}