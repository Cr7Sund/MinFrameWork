using Cr7Sund.AssetLoader.Api;
using Cr7Sund.Server.Api;

namespace Cr7Sund.Server.UI.Api
{
    public interface IUIContainer : ILoadModule
    {
        /// <summary>
        /// current ui containers stack count
        /// </summary>
        int OperateNum { get; }
    }

}
