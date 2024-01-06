using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Server.Impl
{
    public class GameNode : ModuleNode
    {

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

        public IPromise<INode> Destroy()
        {
            DeInject();
            Dispose();
            return UnloadAsync(this).Then(node =>
            {
                SetActive(false);
                Stop();
                Dispose();
                return node;
            });
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

        protected override void OnDispose()
        {
            _context.InjectionBinder.Unbind<GameNode>();

            base.OnDispose();
        }
    }
}
