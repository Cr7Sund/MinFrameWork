namespace Cr7Sund.Server.UI.Api
{
    public enum UIExceptionType
    {
        /// <summary>
        /// open panel should called from page node
        /// </summary>
        INVALID_PANEL_PARENT,
        /// <summary>
        /// open page should called from page node
        /// </summary>
        INVALID_PAGE_PARENT,
        /// <summary>
        /// cant back with no pages
        /// </summary>
        INVALID_BACK,
        /// <summary>
        /// there are nothing to back
        /// </summary>
        NO_LEFT_BACK,
        /// <summary>
        /// cant hide again in transition
        /// </summary>
        HIDE_IN_TRANSITION,
        /// <summary>
        /// try to unload but not load at first
        /// </summary>
        no_load_promise,
    }
}