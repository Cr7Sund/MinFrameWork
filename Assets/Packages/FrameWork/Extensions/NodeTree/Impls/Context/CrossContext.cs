﻿using System.Collections.Generic;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class CrossContext : Context, ICrossContext
    {
        protected ICrossContextInjectionBinder _crossContextInjectionBinder;

        public override IInjectionBinder InjectionBinder
        {
            get
            {
                return _crossContextInjectionBinder;
            }
        }


        public CrossContext()
        {
            _crossContextInjectionBinder = new CrossContextInjectionBinder();
        }


        public sealed override void AddContext(IContext context)
        {
            base.AddContext(context);
            if (context is ICrossContext crossContext)
            {
                AssignCrossContext(crossContext);
            }
        }
        public sealed override void RemoveContext(IContext context)
        {
            if (context is ICrossContext crossContext)
            {
                RemoveCrossContext(crossContext);
            }
            base.RemoveContext(context);
        }

        private void AssignCrossContext(ICrossContext childContext)
        {
            AssertUtil.NotNull(_crossContextInjectionBinder.CrossContextBinder, NodeTreeExceptionType.EMPTY_CROSS_CONTEXT);

            if (childContext.InjectionBinder is CrossContextInjectionBinder childContextBinder)
            {
                if (childContextBinder.CrossContextBinder == null)
                {
                    childContextBinder.CrossContextBinder = new CrossContextInjectionBinder();
                }
                childContextBinder.CrossContextBinder.CopyFrom(_crossContextInjectionBinder.CrossContextBinder);
            }
        }

        private void RemoveCrossContext(ICrossContext childContext)
        {

        }


    }
}
