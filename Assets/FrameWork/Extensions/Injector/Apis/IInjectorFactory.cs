/**
 * @interfaceCr7Sund.Framework.Api.IInjectorFactory
 * 
 * Interface for the Factory that instantiates all instances.
 * 
 * @seeCr7Sund.Framework.Api.IInjector
 */

namespace Cr7Sund.Framework.Api
{
    public interface IInjectorFactory
    {
        /// Request instantiation based on the provided binding
        object Get( IInjectionBinding binding);

        /// Request instantiation based on the provided binding and an array of Constructor arguments
        object Get( IInjectionBinding binding, object[] args);
    }
}

