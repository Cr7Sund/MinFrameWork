namespace Cr7Sund.NodeTree.Impl
{
    public class ModuleNode : UpdateNode
    {
        protected ControllerModule _controllerModule;


        public ModuleNode(IAssetKey assetKey) : base(assetKey)
        {
        }


        public void AssignControllerModule(ControllerModule controllerModule)
        {
            _controllerModule = controllerModule;
        }

        protected override PromiseTask OnPreloadAsync(UnsafeCancellationToken cancellation)
        {
            return _controllerModule.PreLoadAsync(cancellation);
        }

        protected override PromiseTask OnLoadAsync(UnsafeCancellationToken cancellation)
        {
            return _controllerModule.LoadAsync(cancellation);
        }

        protected override PromiseTask OnUnloadAsync(UnsafeCancellationToken cancellation)
        {
            return _controllerModule.UnloadAsync(cancellation);
        }

        public sealed override void Inject()
        {
            if (IsInjected)
                return;

            base.Inject();

            // it should be OnInject
            // but we need to force it will be invoked 
            _controllerModule.AssignContext(_context);
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

        public override PromiseTask OnStart(UnsafeCancellationToken cancellation)
        {
            return _controllerModule.Start(cancellation);
        }

        public override async PromiseTask OnStop()
        {
            await _controllerModule.Stop();
        }

        public override PromiseTask OnEnable()
        {
            return _controllerModule.Enable();
        }

        public override PromiseTask OnDisable(bool closeImmediately )
        {
            return _controllerModule.Disable(closeImmediately);
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
    }
}