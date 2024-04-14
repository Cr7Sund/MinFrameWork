using System.Threading;
using Cr7Sund.AssetLoader.Api;

namespace Cr7Sund.AssetLoader.Impl
{
    public class EditorResourcesLoader : IAssetLoader
    {
        public bool IsInit => true;


        public void Init()
        {
        }
        public void Dispose()
        {
        }



        public void Unload(IAssetPromise handler)
        {

        }

        public void CancelLoad(IAssetPromise handler)
        {
        }

        PromiseTask IAssetLoader.Init()
        {
            return PromiseTask.CompletedTask;
        }


        PromiseTask<T> IAssetLoader.Load<T>(IAssetKey key)
        {
            return new PromiseTask<T>(default(T), 0);
        }

        PromiseTask<T> IAssetLoader.LoadAsync<T>(IAssetKey key)
        {
            return new PromiseTask<T>(default(T), 0);
        }

        public void Unload(IAssetKey handler)
        {
        }

        public void RegisterCancelLoad(IAssetKey handler, CancellationToken cancellation)
        {
        }
    }
}