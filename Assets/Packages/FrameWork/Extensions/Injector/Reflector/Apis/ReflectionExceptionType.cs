namespace Cr7Sund.Framework.Api
{
    public enum ReflectionExceptionType
    {
        /// The reflector requires a constructor, which Interfaces don't provide.
        CANNOT_REFLECT_INTERFACE,

        /// The reflector is not allowed to inject into private/protected setters.
        CANNOT_INJECT_INTO_NONPUBLIC_SETTER,
        // The reflector is not allowed to post construct many times
        CANNOT_POST_CONSTRUCTS
    }
}
