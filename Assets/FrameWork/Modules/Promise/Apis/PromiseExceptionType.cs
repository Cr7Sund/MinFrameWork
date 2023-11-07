
namespace Cr7Sund.Framework.Api
{
    public enum PromiseExceptionType
    {
        Valid_STATE,
        // No promise for race been provided , the result is undefined will be fulfilled or rejected
        EMPTY_PROMISE_RACE,
        // No promise for any been provided , the result is undefined will be fulfilled or rejected
        EMPTY_PROMISE_ANY
    }
}