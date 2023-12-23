using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
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
            ((Context)context).MapBindings();
            _contexts.Add(context);
        }
        public virtual void RemoveContext(IContext context)
        {
            AssertUtil.IsTrue(_contexts.Contains(context));
            ((Context)context).UnMappedBindings();
            _contexts.Remove(context);
        }

        public abstract void MapBindings();
        public abstract void UnMappedBindings();

        public void Dispose()
        {
            _contexts.Clear();
            _contexts = null;
            InjectionBinder.Dispose();
        }
    }
}
