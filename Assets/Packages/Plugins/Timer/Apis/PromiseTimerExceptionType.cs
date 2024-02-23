namespace Cr7Sund.Package.Api
{
    public enum PromiseTimerExceptionType
    {
        CANCEL_TIMER,
        /// <summary>
        /// can add a timer when its duration is less than zero
        /// </summary>
        INVALID_DURATION,
    }
}
