using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Server.Impl
{
    public class GameNode : Node
    {
        internal IControllerModule ControllerModule;

        public void Run()
        {
            Inject();
            Init();
            LoadAsync(this).Then(_ =>
            {
                Start();
                SetActive(true);
            });
        }

        protected override IPromise<INode> OnLoadAsync(INode content)
        {
            var promise = Promise<INode>.Resolved(content)
             .Then(ControllerModule.LoadAsync);

            return promise;
        }

        internal void AssignContext(IContext context)
        {
            _context = context;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _context.InjectionBinder.Bind<GameNode>().To(this);
        }
    }
}
