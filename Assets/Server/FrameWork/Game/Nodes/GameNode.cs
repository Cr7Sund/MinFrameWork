using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.Apis;
namespace Cr7Sund.Server.Impl
{
    public class GameNode : ModuleNode
    {
        [Inject] public ISceneModule SceneModule;
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

        public override void Inject()
        {
            if (IsInjected)
                return;

            _context.InjectionBinder.Bind<GameNode>().To(this);
            base.Inject();
            _context.InjectionBinder.Injector.Inject(this);
        }

        public override void DeInject()
        {
            if (!IsInjected)
                return;

            _context.InjectionBinder.Injector.Uninject(this);
            base.DeInject();
            _context.InjectionBinder.Unbind<GameNode>();
        }

    }
}
