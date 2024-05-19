namespace Cr7Sund.AssetLoader.Api
{
    public enum AssetLoaderExceptionType
    {
        /// <summary>
        /// try to access result when fail
        /// </summary>
        fail_state,
        /// <summary>
        /// try to access result when cancel
        /// </summary>
        cancel_state,
        /// <summary>
        /// try to access result when pending
        /// </summary>
        pending_state,
        /// <summary>
        /// try to access result when not done
        /// </summary>
        no_done_State,
        /// <summary>
        /// try to access result when already recycle
        /// </summary>
        recycle_State,
        /// <summary>
        /// still left handles when try to dispose AsyncOperationHandle
        /// </summary>
        Left_UnResolved_Handlers,
        /// <summary>
        /// try to active unloaded scene
        /// </summary>
        ACTIVE_UNLOADED_SCENE,
        /// <summary>
        /// try to unload twice or more 
        /// </summary>
        unload_duplicate,
    }
}