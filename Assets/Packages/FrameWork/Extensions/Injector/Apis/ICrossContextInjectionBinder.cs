/**
 * @class Cr7Sund.Framework.Api.CrossContextInjectionBinder
 *
 * A special version of InjectionBinder that allows shared injections across multiple Contexts.
 *
 * @see Cr7Sund.Framework.Api.IInjectionBinder
 */


namespace Cr7Sund.Package.Api
{
    public interface ICrossContextInjectionBinder : IInjectionBinder
    {
        /// <summary>
        ///     Cross-context Injection Binder is shared across all child contexts
        /// </summary>
        IInjectionBinder CrossContextBinder { get; set; }
    }
}
