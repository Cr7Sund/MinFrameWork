using Cr7Sund.Framework.Api;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUISequence
    {
        IPromise Open(ViewContent openUiKey);
    }
}