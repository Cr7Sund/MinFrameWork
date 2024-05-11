using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Context : IContext
    {
        private List<IContext> _contexts;

        public virtual IInjectionBinder InjectionBinder { get; private set; }


        public Context()
        {
            _contexts = new List<IContext>();
            InjectionBinder = new InjectionBinder();
        }


        public virtual void AddContext(IContext context)
        {
            AssertUtil.IsFalse(_contexts.Contains(context));
            _contexts.Add(context);
        }
        public virtual void RemoveContext(IContext context)
        {
            AssertUtil.IsTrue(_contexts.Contains(context));
            _contexts.Remove(context);
        }

        public virtual void Dispose()
        {
            AssertUtil.LessOrEqual(_contexts.Count, 0);
            _contexts = null;
            InjectionBinder.Dispose();
            InjectionBinder = null;
        }

        public abstract void AddComponents(INode self);
        public abstract void RemoveComponents();
    }
}
