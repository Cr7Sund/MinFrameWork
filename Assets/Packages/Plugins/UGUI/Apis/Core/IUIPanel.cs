using Cr7Sund.PackageTest.Api;

namespace Cr7Sund.UGUI.Apis
{
    public interface IUIPanel : IUIComponentGetter, IInitialize
    {
        void Hide(bool push);
        void Show(bool push);
        IPromise Animate(bool push);

    }
}