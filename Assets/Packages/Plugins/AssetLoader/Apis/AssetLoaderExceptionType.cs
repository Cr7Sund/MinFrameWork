namespace Cr7Sund.AssetLoader.Api
{
    public enum AssetLoaderExceptionType
    {
        /// <summary>
        /// try to access result when fail
        /// </summary>
        fail_state,
        /// <summary>
        /// try to access result when pending
        /// </summary>
        pending_state,
        /// <summary>
        /// try to access result when not done
        /// </summary>
        no_done_State,
    }
}