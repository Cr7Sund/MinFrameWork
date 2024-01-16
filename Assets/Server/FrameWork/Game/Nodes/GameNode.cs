using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
namespace Cr7Sund.Server.Impl
{
    public class GameNode : ModuleNode
    {

        public void Run()
        {
            AssertUtil.NotNull(_context, NodeTreeExceptionType.EMPTY_CONTEXT);
            
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

            return UnloadAsync(this).Then(node =>
            {
                SetActive(false);
                Stop();
                Dispose();
                return node;
            });
        }

        public void AssignContext(IContext context)
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
