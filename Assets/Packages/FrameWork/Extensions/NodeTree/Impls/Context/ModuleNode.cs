using Cr7Sund.Framework.Api;
using Cr7Sund.NodeTree.Api;

namespace Cr7Sund.NodeTree.Impl
{
    public class ModuleNode : UpdateNode
    {
        private IControllerModule _controllerModule;


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