using Cr7Sund.Package.Api;

namespace Cr7Sund.UGUI.Apis
{
    public interface IUIPanel : IUIComponentGetter, IInitialize
    {
        void Hide(bool push);
        void Show(bool push);
        IPromise Animate(bool push);

    }
}