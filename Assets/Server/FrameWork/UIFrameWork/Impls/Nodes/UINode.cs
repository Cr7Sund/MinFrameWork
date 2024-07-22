using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Server.Impl;
using System;
using Object = UnityEngine.Object;
using UnityEngine;

namespace Cr7Sund.Server.UI.Impl
{
    //node as Controller
    public abstract class UINode : UpdateNode, IUINode
    {
        private readonly string _panelID;
        [Inject(ServerBindDefine.UIPanelContainer)] private IInstancesContainer _uiContainer;
        [Inject(ServerBindDefine.UIPanelUniqueContainer)] private IUniqueInstanceContainer _uiUniqueContainer;
        [Inject(ServerBindDefine.UILogger)] protected IInternalLog _log;

        public IUIView View { get; private set; }
        public string PanelID => _panelID;
        public bool IsTransitioning { get; private set; }
        public IUIController Controller { get; private set; }

        protected UINode(IAssetKey assetKey) : base(assetKey)
        {
            _panelID = System.Guid.NewGuid().ToString();
        }

        public UINode(IAssetKey assetKey, IUIView uiView, IUIController uiController) : this(assetKey)
        {
            View = uiView;
            Controller = uiController;
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

        public async PromiseTask Enter(bool push, IUINode partnerView, bool playAnimation, UnsafeCancellationToken cancellation)
        {
            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                View.Enable(Parent);
            }
            await Controller.Enable();
            await View.EnterRoutine(push, partnerView, playAnimation, cancellation);
        }

        public async PromiseTask Exit(bool push, IUINode partnerView, bool playAnimation, bool closeImmediately, UnsafeCancellationToken cancellation)
        {
            View.Disable();
            await Controller.Disable(closeImmediately);
            await View.ExitRoutine(push, partnerView, playAnimation, cancellation);
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

        protected override async PromiseTask OnPreloadAsync(UnsafeCancellationToken cancellation)
        {
            var uiKey = Key as UIKey;
            var prepareTask = Controller.Prepare(cancellation, uiKey.Intent);
            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                if (uiKey.UniqueInstance)
                {
                    await _uiUniqueContainer.LoadAssetAsync<Object>(Key, cancellation);
                }
                else
                {
                    await _uiContainer.LoadAssetAsync<Object>(Key, cancellation);
                }
            }
            await prepareTask;
        }

        protected override async PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation)
        {
            AssertUtil.IsFalse(LoadState == LoadState.Loaded); // handle different situation outside

            var uiKey = Key as UIKey;
            var prepareTask = Controller.Prepare(cancellation, uiKey.Intent);

            if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
            {
                GameObject instance = null;
                if (uiKey.UniqueInstance)
                {
                    instance = await _uiUniqueContainer.CreateInstanceAsync<GameObject>(Key, cancellation);
                }
                else
                {
                    instance = await _uiContainer.InstantiateAsync<GameObject>(Key, PanelID, cancellation);
                }
                await View.OnLoad(instance);
                await prepareTask;
            }
            else
            {
                await View.OnLoad(null);
                await prepareTask;
            }
        }

        protected override async PromiseTask OnUnloadAsync(UnsafeCancellationToken cancellation)
        {
            var uiKey = Key as UIKey;
            if (uiKey.UniqueInstance)
            {
                await _uiUniqueContainer.Unload(Key);
            }
            else
            {
                await _uiContainer.ReturnInstance(PanelID, Key);
            }
            await base.OnUnloadAsync(cancellation);
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

        public override async PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            try
            {
                if (MacroDefine.IsMainThread && UnityEngine.Application.isPlaying)
                {
                    View.Start(Parent);
                }
                await Controller.Start(cancellation);
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
                await Enter(isPush, exitPage, enterUIKey.PlayAnimation, AddCancellation.Token);
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

        public override async PromiseTask OnDisable(bool closeImmediately)
        {
            UIKey exitUIkey = Key as UIKey;
            bool isPush = exitUIkey.IsPush;
            IUINode enterPage = exitUIkey.exitPage;

            bool playAnimation = !closeImmediately && exitUIkey.PlayAnimation;

            try
            {
                IsTransitioning = true;

                await BeforeExit(isPush, enterPage);
                await Exit(isPush, enterPage, playAnimation, closeImmediately, RemoveCancellation.Token);
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
