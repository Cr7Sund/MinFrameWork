/**
 * @interface Cr7Sund.Framework.Api.IContextView
 *
 * The ContextView is the entry point to the application.
 *
 * In a standard MVCSContext setup for Unity3D, it is a MonoBehaviour
 * attached to a GameObject at the very top of of your application.
 * It's most important role is to instantiate and call `Start()` on the Context.
 */

namespace Cr7Sund.Framework.Api
{
    public interface IContextView : IView
    {
        /// Get and set the Context
        IContext Context { get; set; }
    }
}
