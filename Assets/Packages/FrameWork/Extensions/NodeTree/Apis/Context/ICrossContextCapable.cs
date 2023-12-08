using Cr7Sund.Framework.Api;
namespace Cr7Sund.NodeTree.Api
{
    public interface ICrossContextCapable
    {
        /// All cross-context capable contexts must implement an injectionBinder
        ICrossContextInjectionBinder InjectionBinder { get; }
        /// Set and get the shared system bus for communicating across contexts
        IDispatcher CrossContextDispatcher { get; }

    }
}
