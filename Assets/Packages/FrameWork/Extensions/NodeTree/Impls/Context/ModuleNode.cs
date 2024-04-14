using System.Threading;

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

        public override async PromiseTask PreLoadAsync()
        {
            await base.PreLoadAsync();
            await _controllerModule.PreLoadAsync();
        }

        public override async PromiseTask LoadAsync()
        {
            await base.LoadAsync();
            await _controllerModule.LoadAsync();
        }

        public override async PromiseTask UnloadAsync()
        {
            await base.UnloadAsync();
            await _controllerModule.UnloadAsync();
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

        public override PromiseTask OnStart()
        {
            return _controllerModule.Start();
        }

        public override async PromiseTask OnStop()
        {
            await _controllerModule.Stop();
        }

        public override PromiseTask OnEnable()
        {
            return _controllerModule.Enable();
        }

        public override PromiseTask OnDisable()
        {
            return _controllerModule.Disable();
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

        public override void RegisterAddTask(CancellationToken cancellationToken)
        {
            base.RegisterAddTask(cancellationToken);
            _controllerModule.RegisterAddTask(cancellationToken);
        }

        public override void RegisterRemoveTask(CancellationToken cancellationToken)
        {
            base.RegisterRemoveTask(cancellationToken);
            _controllerModule.RegisterRemoveTask(cancellationToken);
        }
    }
}