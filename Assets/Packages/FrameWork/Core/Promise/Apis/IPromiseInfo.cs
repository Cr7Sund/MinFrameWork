namespace Cr7Sund.Framework.Api
{
    /// <summary>
    ///     Used to list information of pending promises.
    /// </summary>
    public interface IPromiseInfo
    {
        /// <summary>
        ///     Id of the promise.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Human-readable name for the promise.
        /// </summary>
        object Name { get; }
    }
}
