using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class Context : IContext
    {
        private IInjectionBinder _injectionBinder;
        private List<IContext> _contexts;
        
        public IInjectionBinder InjectionBinder
        {
            get
            {
                return _injectionBinder;
            }
            set
            {
                _injectionBinder = value;
            }
        }

        public virtual void AddContext(IContext context)
        {
            AssertUtil.IsFalse(_contexts.Contains(context));
            _contexts.Add(context);
        }
        public virtual  void RemoveContext(IContext context)
        {
            AssertUtil.IsTrue(_contexts.Contains(context));
            _contexts.Remove(context);
        }

        public void Dispose()
        {
            _contexts.Clear();
            _contexts = null;
            _injectionBinder.Dispose();
        }
    }
}
