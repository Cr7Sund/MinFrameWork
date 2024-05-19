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




        PromiseTask IAssetLoader.Init()
        {
            return PromiseTask.CompletedTask;
        }

        PromiseTask<T> IAssetLoader.Load<T>(IAssetKey key)
        {
            return new PromiseTask<T>(default(T), 0);
        }

        PromiseTask<T> IAssetLoader.LoadAsync<T>(IAssetKey key, UnsafeCancellationToken cancellation)
        {
            return new PromiseTask<T>(default(T), 0);
        }

        public PromiseTask Unload(IAssetKey handler)
        {
            return PromiseTask.CompletedTask;
        }

        public PromiseTask CancelLoad(IAssetKey handler)
        {
            return PromiseTask.CompletedTask;
        }


        public void LateUpdate(int millisecond)
        {
        }

        public PromiseTask DestroyAsync()
        {
            return PromiseTask.CompletedTask;
        }
    }
}