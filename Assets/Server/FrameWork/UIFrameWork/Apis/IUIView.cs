using Cr7Sund.PackageTest.Api;  
using Cr7Sund.UGUI.Apis;     

/**
 * @class Cr7Sund.Framework.Api.IView
 *
 * Monobehaviours must implement this interface in order to be injectable.
 *
 * To contact the Context, the View must be able to find it. View handles this
 * with bubbling.
 */
namespace Cr7Sund.Server.UI.Api
{
    public interface IUIView
    {
        void Hide(bool push);
        void Show(bool push);
        IPromise Animate(bool push);
    }
}
