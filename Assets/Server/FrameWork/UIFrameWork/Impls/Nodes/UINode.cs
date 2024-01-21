using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.UGUI.Apis;

namespace Cr7Sund.Server.UI.Impl
{
    public class UINode : UpdateNode, IUIView
    {

        [Inject] private IAssetLoader _assetLoader;
        [Inject] private IPoolBinder _poolBinder;
        private ViewContent _viewContent;

        public IUIPanel View { get; set; }
        public string PageId { get; set; }
        public bool IsTransitioning { get; private set; }

        public IUIController VM => throw new System.NotImplementedException();
        public bool Visible => throw new System.NotImplementedException();


        public void AssignViewContent(ViewContent content) => _viewContent = content;
        
        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var uiNode = content as UINode;
            var uiKey = uiNode.Key;
            var intent = uiNode._viewContent.intent;

            return VM.Prepare(intent)
                         .Then(() => uiNode._viewContent.loadAsync ? _assetLoader.InstantiateAsync(uiKey)
                                             : _assetLoader.Instantiate(uiKey))
                         .Then(panel =>
                              {
                                  //   Getter = panel as IUIComponentGetter;
                                  // View = panel as UIView;
                                  return content;
                              });
        }

        public IPromise BeforeExit(bool push, IUIView enterPage)
        {
            IsTransitioning = true;

            View.Show(push);

            if (push)
                return VM.WillPushExit();
            else
                return VM.WillPopExit();
        }
        public IPromise BeforeEnter(bool push, IUIView enterPage)
        {
            IsTransitioning = true;

            View.Hide(push);

            if (push)
                return VM.WillPushEnter();
            else
                return VM.WillPushEnter();
        }
        public IPromise Enter(bool push, IUIView partnerView)
        {
            return View.Animate(push);
        }
        public IPromise Exit(bool push, IUIView partnerView)
        {
            return View.Animate(push);
        }
        public IPromise AfterEnter(bool push, IUIView exitPage)
        {
            IsTransitioning = false;
            View.Show(push);

            if (push)
                return VM.DidPushEnter();
            else
                return VM.DidPopEnter();
        }
        public IPromise AfterExit(bool push, IUIView enterPage)
        {
            View.Hide(push);
            IsTransitioning = false;

            if (push)
                return VM.DidPushExit();
            else
                return VM.DidPopExit();
        }


        #region  LifeCycle
        public override void Inject()
        {
            base.Inject();
            _context.InjectionBinder.Injector.Inject(VM);
        }

        public override void DeInject()
        {
            _context.InjectionBinder.Injector.Uninject(VM);
            base.DeInject();
        }

        protected override void OnInit()
        {
            base.OnInit();
            // Getter.Init();
        }
        protected override void OnStart()
        {
            VM.Start();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            VM.Enable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            VM.Disable();
        }

        protected override void OnStop()
        {
            base.OnStop();
            VM.Stop();
        }
        protected override void OnDispose()
        {
            base.OnDispose();
            // VM();
        }

        #endregion
    }
}