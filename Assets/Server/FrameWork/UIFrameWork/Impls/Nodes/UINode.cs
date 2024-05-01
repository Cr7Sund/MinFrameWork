using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.FrameWork.Util;
using Object = UnityEngine.Object;
using System.Threading;
using Cr7Sund.Server.Impl;
using System;

namespace Cr7Sund.Server.UI.Impl
{
    public abstract class UINode : UpdateNode, IUINode
    {
        [Inject(ServerBindDefine.UIPanelContainer)] private IUniqueInstanceContainer _uiContainer;
        [Inject] private IPanelModule _panelModule;
        [Inject(ServerBindDefine.UILogger)] protected IInternalLog _log;

        public IUIView View { get; private set; }
        public string PageId { get; set; }
        public bool IsTransitioning { get; private set; }
        public IUIController Controller { get; private set; }


        protected UINode(IAssetKey assetKey) : base(assetKey)
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
            View.BeforeExit();

            if (push)
                return Controller.WillPushExit();
            else
                return Controller.WillPopExit();
        }

        public PromiseTask BeforeEnter(bool push, IUINode enterPage)
        {
            View.BeforeEnter();

            if (push)
                return Controller.WillPushEnter();
            else
                return Controller.WillPushEnter();
        }

        public async PromiseTask Enter(bool push, IUINode partnerView, bool playAnimation)
        {
            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                View.Enable(Parent);
            }
            await Controller.Enable();
            await View.EnterRoutine(push, partnerView, playAnimation);
        }

        public async PromiseTask Exit(bool push, IUINode partnerView, bool playAnimation)
        {
            View.Disable();
            await Controller.Disable();
            await View.ExitRoutine(push, partnerView, playAnimation);
        }

        public async PromiseTask AfterEnter(bool push, IUINode exitPage)
        {
            View.AfterEnter();

            if (push)
                await Controller.DidPushEnter();
            else
                await Controller.DidPopEnter();

        }

        public async PromiseTask AfterExit(bool push, IUINode enterPage)
        {
            View.AfterExit();

            if (push)
                await Controller.DidPushExit();
            else
                await Controller.DidPopExit();
        }

        protected override async PromiseTask OnPreloadAsync()
        {
            var uiKey = Key as UIKey;
            var prepareTask = Controller.Prepare(uiKey.Intent);
            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                await _uiContainer.LoadAssetAsync<Object>(Key);
            }
            await prepareTask;
        }

        protected override async PromiseTask OnLoadAsync()
        {
            AssertUtil.IsFalse(LoadState == LoadState.Loaded); // handle different situation outside

            var uiKey = Key as UIKey;
            var prepareTask = Controller.Prepare(uiKey.Intent);

            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                var instance = await _uiContainer.CreateInstanceAsync<Object>(Key);
                // Attention : the below chain load task is called from addressables
                await View.OnLoad(instance as UnityEngine.GameObject);
                await prepareTask;
            }
            else
            {
                await View.OnLoad(null);
                await prepareTask;
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
            try
            {
                if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
                {
                    View.Start(Parent);
                }
                await Controller.Start();
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    _log.Error(ex);
                }
            }
        }

        public override async PromiseTask OnEnable()
        {
            UIKey enterUIKey = Key as UIKey;
            bool isPush = enterUIKey.IsPush;
            IUINode exitPage = enterUIKey.exitPage;

            try
            {
                IsTransitioning = true;

                await BeforeEnter(isPush, exitPage);
                await Enter(isPush, exitPage, enterUIKey.PlayAnimation);
                await AfterEnter(push: isPush, exitPage);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    _log.Error(ex);
                }
            }
            finally
            {
                IsTransitioning = false;
            }
        }

        protected override void OnUpdate(int milliseconds)
        {
            base.OnUpdate(milliseconds);
            View.Update(milliseconds);
        }

        public override async PromiseTask OnDisable()
        {
            UIKey exitUIkey = Key as UIKey;
            bool isPush = exitUIkey.IsPush;
            IUINode enterPage = exitUIkey.exitPage;

            try
            {
                IsTransitioning = true;

                await BeforeExit(isPush, enterPage);
                await Exit(isPush, enterPage, exitUIkey.PlayAnimation);
                await AfterExit(isPush, enterPage);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    _log.Error(ex);
                }
            }
            finally
            {
                IsTransitioning = false;
            }
        }

        public override async PromiseTask OnStop()
        {
            try
            {
                View.Stop();
                await Controller.Stop();
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    throw;
                }
                else
                {
                    _log.Error(ex);
                }
            }
        }

        protected override void OnDispose()
        {
            View.Dispose();
        }
        #endregion
    }
}
