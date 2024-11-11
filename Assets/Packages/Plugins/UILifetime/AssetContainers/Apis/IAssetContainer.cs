using System;
using Cr7Sund.LifeTime;
using Object = UnityEngine.Object;
namespace Cr7Sund.LifeTime
{
    public interface IAssetContainer : IDisposable, ILifeCycleAwareComponent
    {
        PromiseTask Unload(IAssetKey key);
        PromiseTask UnloadAll();
    }
}
