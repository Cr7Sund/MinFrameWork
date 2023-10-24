/**
 * @class Cr7Sund.Framework.Api.CrossContextInjectionBinder
 * 
 * A special version of InjectionBinder that allows shared injections across multiple Contexts.
 * 
 * @see Cr7Sund.Framework.Api.IInjectionBinder
 */


namespace Cr7Sund.Framework.Api
{
    public interface ICrossContextInjectionBinder : IInjectionBinder
    {
        //Cross-context Injection Binder is shared across all child contexts
        IInjectionBinder CrossContextBinder { get; set; }
    }
}
