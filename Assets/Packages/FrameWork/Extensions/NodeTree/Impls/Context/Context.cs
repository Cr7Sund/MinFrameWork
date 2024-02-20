﻿using System.Collections.Generic;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Impl;
using Cr7Sund.PackageTest.Util;
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
            if (!_contexts.Contains(context))
            {
                _contexts.Add(context);
            }
        }
        public virtual void RemoveContext(IContext context)
        {
            AssertUtil.IsTrue(_contexts.Contains(context));
            _contexts.Remove(context);
        }

        public void Dispose()
        {
            _contexts.Clear();
            _contexts = null;
            InjectionBinder.Dispose();
        }

        public abstract void AddComponents();
        public abstract void RemoveComponents();
    }
}
