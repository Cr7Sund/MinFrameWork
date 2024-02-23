using Cr7Sund.Package.Api;
using Cr7Sund.UGUI.Impls;

namespace Cr7Sund.UGUI.Apis
{
    public interface IUIPanel : IUIComponentGetter, IInitialize
    {
        UITransitionAnimation GetAnimation(bool push, bool enter, IAssetKey partnerTransitionUI);
    }
}