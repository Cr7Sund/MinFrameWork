/**
 * @interface Cr7Sund.Framework.Api.IContext
 *
 * A Context is the entry point to the binding framework.
 *
 * Implement this interface to create the binding context suitable for your application.
 *
 * In a typical Unity3D setup, an extension of MVCSContext should be instantiated from the ContextView.
 */

using System;
namespace Cr7Sund.Framework.Api
{
    public interface IContext : IBinder
    {
        // Kicks off the internal Context binding/instantiation mechanisms
        IContext Start();

        void Launch();

        // Register a new context to this
        IContext AddContext(IContext context);

        // Remove a context from this one
        IContext RemoveContext(IContext context);

        /// Register a view with this context
        void AddView(object view);

        /// Remove a view from this context
        void RemoveView(object view);

        /// Get the ContextView
        object GetContextView();
    }

    [Flags]
    public enum ContextStartupFlags
    {
        /// Context will map bindings and launch automatically (default).
        AUTOMATIC = 0,
        /// Context startup will halt after Core bindings are mapped, but before instantiation or any custom bindings.
        /// If this flag is invoked, the developer must call context.Start()
        MANUAL_MAPPING = 1,
        /// Context startup will halt after all bindings are mapped, but before firing ContextEvent.START (or the analogous Signal).
        /// If this flag is invoked, the developer must call context.Launch()
        MANUAL_LAUNCH = 2
    }
}
