namespace Cr7Sund.Framework.Api
{
    public enum DispatcherExceptionType
    {
        /// Injector Factory not found
        NULL_FACTORY,

        /// EventDispatcher can't map something that isn't a delegate
        ILLEGAL_CALLBACK_HANDLER,
        // Event callbacks must have either one or no arguments
        OUT_OF_ARGUMENT_EVENT
    }
}
