using System;
using System.Threading;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Api
{
    public interface IUniqueInstanceContainer : IAssetContainer
    {
        PromiseTask<T> CreateInstance<T>(IAssetKey key) where T : Object;
        PromiseTask<T> CreateInstanceAsync<T>(IAssetKey key, CancellationToken cancellation) where T : Object;
        PromiseTask CancelLoad(IAssetKey key, CancellationToken token);
    }
}
