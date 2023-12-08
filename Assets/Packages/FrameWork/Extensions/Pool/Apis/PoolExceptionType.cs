namespace Cr7Sund.Framework.Api
{
    public enum PoolExceptionType
    {

        /// POOL HAS OVERFLOWED ITS LIMIT
        OVERFLOW,
        /// the instantiated instance has reach the limit
        REACH_MAX_LIMIT,

        /// ATTEMPT TO ADD AN INSTANCE OF DIFFERENT TYPE TO A POOL
        TYPE_MISMATCH,

        /// A POOL HAS NO INSTANCE PROVIDER
        NO_INSTANCE_PROVIDER,
        // NO_INSTANCE_TO_CREATE, it will cause circular 
        NO_INSTANCE_TO_CREATE
    }
}
