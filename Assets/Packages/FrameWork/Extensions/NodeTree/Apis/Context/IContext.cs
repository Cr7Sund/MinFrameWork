﻿using System;
using Cr7Sund.Package.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface IContext : IDisposable
    {
        IInjectionBinder InjectionBinder { get;  }
        /// Register a new context to this one
        void AddContext(IContext context);

        /// Remove a context from this one
        void RemoveContext(IContext context);

        void AddComponents(INode self);
        void RemoveComponents();
    }
}
