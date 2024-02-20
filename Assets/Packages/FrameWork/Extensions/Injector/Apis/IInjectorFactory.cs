/**
 * @interfaceCr7Sund.Framework.Api.IInjectorFactory
 *
 * Interface for the Factory that instantiates all instances.
 *
 * @seeCr7Sund.Framework.Api.IInjector
 */

namespace Cr7Sund.Package.Api
{
    public interface IInjectorFactory
    {
        /// <summary>  Request instantiation based on the provided binding </summary>
        object Get(IInjectionBinding binding);

        /// <summary>  Request instantiation based on the provided binding and an array of Constructor arguments </summary>
        object Get(IInjectionBinding binding, object[] args);
    }
}
