using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    public class ModuleNode : UpdateNode
    {
        protected IControllerModule _controllerModule;


        public ModuleNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        public void AssignControllerModule(ControllerModule controllerModule)
        {
            _controllerModule = controllerModule;
        }


        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            return base.OnLoadAsync(content).Then(_controllerModule.LoadAsync);
        }

        protected override IPromise<INode> OnUnloadAsync(INode content)
        {
            return base.OnUnloadAsync(content).Then(_controllerModule.UnloadAsync);
        }

        public sealed override void Inject()
        {
            if (IsInjected)
                return;

            base.Inject();

            // it should be OnInject
            // but we need to force it will be invoked 
            ((ControllerModule)_controllerModule).AssignContext(_context);
            _controllerModule.Inject();
        }

        public sealed override void Deject()
        {
            if (!IsInjected)
                return;

            base.Deject();

            // it should be OnDeInject
            // but we need to force it will be invoked 
            _controllerModule.Deject();
        }

        protected override void OnStart()
        {
            base.OnStart();

            _controllerModule.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();

            _controllerModule.Stop();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _controllerModule.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _controllerModule.Disable();
        }

        protected override void OnUpdate(int milliseconds)
        {
            base.OnUpdate(milliseconds);

            _controllerModule.Update(milliseconds);
        }

        protected override void OnLateUpdate(int milliseconds)
        {
            base.OnLateUpdate(milliseconds);

            _controllerModule.LateUpdate(milliseconds);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}