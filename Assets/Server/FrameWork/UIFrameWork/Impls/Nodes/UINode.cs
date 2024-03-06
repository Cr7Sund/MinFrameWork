using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using UnityEngine;
using Cr7Sund.Server.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UINode : UpdateNode, IUINode
    {
        [Inject] private IAssetInstanceContainer _uiContainer;

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
                var assetPromise = _uiContainer.GetAssetAsync(content.Key);
                loadPromise = assetPromise.Then(_ => { });
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

            if (_uiContainer.ContainsAsset(Key))
            {
                var assetPromise = uiKey.LoadAsync ? _uiContainer.CreateInstanceAsync<GameObject>(uiKey)
                                                               : _uiContainer.CreateInstance<GameObject>(uiKey);
                return assetPromise.Then(_ => Promise<INode>.Resolved(content));
            }
            else
            {
                IPromise loadPromise = null;
                IPromise preparePromise = Controller.Prepare(uiKey.Intent);

                if (Application.isPlaying)
                {
                    var assetPromise = uiKey.LoadAsync ? _uiContainer.CreateInstanceAsync<GameObject>(uiKey)
                                                       : _uiContainer.CreateInstance<GameObject>(uiKey);
                    loadPromise = assetPromise.Then(_ => { });
                }
                else
                {
                    loadPromise = base.OnLoadAsync(content)
                                     .Then(_ => { });
                }

                return Promise.All(preparePromise, loadPromise)
                    .Then(() => Promise<INode>.Resolved(content));
            }
        }

        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            _uiContainer.Unload(Key);
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
            _context.InjectionBinder.Injector.Deject(View);
            _context.InjectionBinder.Injector.Deject(Controller);
        }

        protected override void OnInit()
        {
            // duplicate
            // and the controller should not control anything 
            // that has not instantiated
        }

        protected override void OnStart()
        {
            if (_uiContainer.TryGetInstance<GameObject>(Key, out var go))
            {
                View.Start(go, Parent);
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