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
        NO_UNRESOLVED,
        // forbid conversion in fist chain
        CONVERT_FIRST,
        // can not react an released binding , try to do not using at once
        CAN_NOT_REACT_RELEASED,
        // can not react an running binding since using at once, try to use ForceStop
        CAN_NOT_REACT_RUNNING,
    }
}
