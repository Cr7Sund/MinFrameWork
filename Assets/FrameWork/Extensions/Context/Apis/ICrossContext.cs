/**
 * @interface Cr7Sund.Framework.Api.ICrossContextCapable
 * 
 * API for allowing Contexts to register across the Context border.
 * 
 * Implement this interface to create a binding context that can communicate across Context boundaries.
 * 
 * MVCSContext inherits CrossContext to obtain important capabilities, such as shared injections.
 * 
 * @see Cr7Sund.Framework.Api.IInjectionBinding
 */



namespace Cr7Sund.Framework.Api
{
    public interface ICrossContext
    {
        /// Add cross-context functionality to a child context being added
        void AssignCrossContext(ICrossContext childContext);

        /// Clean up cross-context functionality from a child context being removed
        void RemoveCrossContext(ICrossContext childContext);

        /// Request a component from the context (might be useful in certain cross-context situations)
        /// This is technically a deprecated methodology. Bind using CrossContext() instead.
        object GetComponent<T>();

        /// Request a component from the context (might be useful in certain cross-context situations)
        /// This is technically a deprecated methodology. Bind using CrossContext() instead.
        object GetComponent<T>(object name);

        /// All cross-context capable contexts must implement an injectionBinder
        ICrossContextInjectionBinder InjectionBinder { get; set; }

        /// Set and get the shared system bus for communicating across contexts
        IEventDispatcher CrossContextDispatcher { get; set; }

    }

    public enum ContextKeys
    {
        /// Marker for the named Injection of the Context
        CONTEXT,
        /// Marker for the named Injection of the ContextView
        CONTEXT_VIEW,
        /// Marker for the named Injection of the contextDispatcher
        CONTEXT_DISPATCHER,
        /// Marker for the named Injection of the crossContextDispatcher
        CROSS_CONTEXT_DISPATCHER
    }
}