using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using UnityEngine;
using Cr7Sund.Server.Api;
using Cr7Sund.FrameWork.Util;
using Object = UnityEngine.Object;
using System.Threading;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.UI.Impl
{
    public class UINode : UpdateNode, IUINode
    {
        [Inject(ServerBindDefine.UIPanelContainer)] private IUniqueInstanceContainer _uiContainer;
        [Inject] private IPanelModule _panelModule;

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


        public PromiseTask BeforeExit(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.BeforeExit();

            if (push)
                return Controller.WillPushExit();
            else
                return Controller.WillPopExit();
        }

        public PromiseTask BeforeEnter(bool push, IUINode enterPage)
        {
            IsTransitioning = true;

            View.BeforeEnter();

            if (push)
                return Controller.WillPushEnter();
            else
                return Controller.WillPushEnter();
        }

        public async PromiseTask Enter(bool push, IUINode partnerView, bool playAnimation)
        {
            await Controller.Enable();
            await View.EnterRoutine(push, partnerView, playAnimation);
        }

        public async PromiseTask Exit(bool push, IUINode partnerView, bool playAnimation)
        {
            try
            {
                await Controller.Disable();
                await View.ExitRoutine(push, partnerView, playAnimation);
            }
            catch (System.Exception ex)
            {
                Console.Error(ex);
                throw;
            }
        }

        public PromiseTask AfterEnter(bool push, IUINode exitPage)
        {
            IsTransitioning = false;
            View.AfterEnter();

            if (push)
                return Controller.DidPushEnter();
            else
                return Controller.DidPopEnter();
        }

        public PromiseTask AfterExit(bool push, IUINode enterPage)
        {
            IsTransitioning = false;
            View.AfterExit();

            if (push)
                return Controller.DidPushExit();
            else
                return Controller.DidPopExit();
        }

        public override async PromiseTask PreLoadAsync()
        {
            if (LoadState == LoadState.Loading
                || LoadState == LoadState.Unloading
                || LoadState == LoadState.Loaded)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            var uiKey = Key as UIKey;
            var prepareTask = Controller.Prepare(uiKey.Intent);
            var preloadTask = base.PreLoadAsync();

            await prepareTask;
            await preloadTask;
        }

        public override async PromiseTask LoadAsync()
        {
            if (LoadState == LoadState.Loading
                || LoadState == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            var uiKey = Key as UIKey;
            try
            {
                if (LoadState != LoadState.Loaded) //already preload
                {
                    await Controller.Prepare(uiKey.Intent);
                }
                await base.LoadAsync();
            }
            catch
            {
                await base.LoadAsync();
                throw;
            }
        }

        protected override async PromiseTask OnPreloadAsync()
        {
            if (Application.isPlaying)
            {
                await _uiContainer.LoadAssetAsync<Object>(Key);
            }
        }

        protected override async PromiseTask OnLoadAsync()
        {
            AssertUtil.IsFalse(LoadState == LoadState.Loaded); // handle different situation outside

            if (MacroDefine.IsMainThread && Application.isPlaying)
            {
                var instance = await _uiContainer.CreateInstanceAsync<Object>(Key);
                View.OnLoad(instance as GameObject);
            }
        }

        protected override PromiseTask OnUnloadAsync()
        {
            _uiContainer.Unload(Key);
            return base.OnUnloadAsync();
        }

        public override void RegisterAddTask(CancellationToken cancellationToken)
        {
            _uiContainer.RegisterCancelLoad(Key, cancellationToken);
            Controller.RegisterAddTask(cancellationToken);
        }

        public override void RegisterRemoveTask(CancellationToken cancellationToken)
        {
            Controller.RegisterRemoveTask(cancellationToken);
        }

        #region LifeCycle
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

        public override async PromiseTask OnStart()
        {
            if (MacroDefine.IsMainThread && Application.isPlaying)
            {
                View.Start(Parent);
            }
            await Controller.Start();
        }

        public override async PromiseTask OnEnable()
        {
            // call after transition
            // and always be called after Start
            // VM.Enable();

            if (MacroDefine.IsMainThread && Application.isPlaying)
            {
                View.Enable(Parent);
            }
            await base.OnEnable();
        }

        protected override void OnUpdate(int milliseconds)
        {
            base.OnUpdate(milliseconds);
            View.Update(milliseconds);
        }

        public override async PromiseTask OnDisable()
        {
            View.Disable();
            await base.OnDisable();
        }

        public override async PromiseTask OnStop()
        {
            View.Stop();
            await Controller.Stop();

            await _panelModule.CloseAll();
        }

        protected override void OnDispose()
        {
            // duplicate
            View.Dispose();
        }
        #endregion
    }
}
