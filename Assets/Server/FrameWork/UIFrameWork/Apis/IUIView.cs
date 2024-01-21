


using Cr7Sund.Framework.Api;
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
        IUIController VM { get; }
        /// <summary>
        /// 是否可见
        /// </summary>
        bool Visible { get; }
        /// <summary>
        /// UI组件获取器
        /// </summary>
        IUIPanel View { get; }
        string PageId { get;set; }


        IPromise BeforeExit(bool push, IUIView partnerView);
        IPromise BeforeEnter(bool push, IUIView partnerView);
        IPromise Exit(bool push,  IUIView partnerView);
        IPromise Enter(bool push, IUIView partnerView);
        IPromise AfterExit(bool push, IUIView enterPage);
        IPromise AfterEnter(bool push, IUIView enterPage);
    }
}
