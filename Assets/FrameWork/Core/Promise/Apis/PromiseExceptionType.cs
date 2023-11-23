namespace Cr7Sund.Framework.Api
{
    public enum PromiseExceptionType
    {
        Valid_STATE,
        // No promise for race been provided , the result is undefined will be fulfilled or rejected
        EMPTY_PROMISE_RACE,
        // No promise for any been provided , the result is undefined will be fulfilled or rejected
        EMPTY_PROMISE_ANY,
        // exception in OnExecuteAsync
        EXCEPTION_ON_ExecuteAsync,
        // there is no promise command to bind
        EMPTY_PROMISE_TOREACT,
        // This version of the function must supply an onResolved.
        // Otherwise there is now way to get the converted value to pass to the resulting promise.
        NO_ONResolved,
        // forbid conversion in fist chain
        CONVERT_FIRST,
    }
}
