using System;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class AsyncLoadable : ILoadAsync
    {
        public LoadState LoadState { get; private set; }


        public virtual async PromiseTask LoadAsync()
        {
            if (LoadState == LoadState.Loading
             || LoadState == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            LoadState = LoadState.Loading;
            try
            {
                await OnLoadAsync();
                _Loaded();
            }
            catch (Exception ex)
            {
                _HandleException(ex);
                throw;
            }
        }

        public virtual async PromiseTask PreLoadAsync()
        {
            if (LoadState == LoadState.Loading
                || LoadState == LoadState.Unloading
                || LoadState == LoadState.Loaded)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            LoadState = LoadState.Loading;
            try
            {
                await OnPreloadAsync();
                _Preloaded();
            }
            catch (Exception ex)
            {
                _HandleException(ex);
                throw;
            }
        }

        public virtual async PromiseTask UnloadAsync()
        {
            if (LoadState == LoadState.Default
            || LoadState == LoadState.Unloading
            || LoadState == LoadState.Unloaded)
            {
                throw new MyException($"Cant UnloadAsync On State: {LoadState}  Loadable: {this} ",
                    NodeTreeExceptionType.UNLOAD_VALID_STATE);
            }

            if (LoadState == LoadState.Loading)
            {
                throw new MyException($"Please handle loading situation outside ",
                      NodeTreeExceptionType.UNLOAD_VALID_STATE);
            }

            LoadState = LoadState.Unloading;
            try
            {
                await OnUnloadAsync();
                _Unloaded();
            }
            catch (Exception ex)
            {
                _HandleException(ex);
                throw;
            }
        }

        public virtual void Dispose()
        {
            // default
            AssertUtil.IsTrue(LoadState == LoadState.Unloaded);

            LoadState = LoadState.Default;
        }

        private void _Loaded()
        {
            LoadState = LoadState.Loaded;
            OnLoaded();
        }
        private void _Preloaded()
        {
            LoadState = LoadState.Loaded;
            OnPreLoaded();
        }
        private void _Unloaded()
        {
            LoadState = LoadState.Unloaded;
            OnUnloaded();
        }
        private void _HandleException(Exception e)
        {
            LoadState = LoadState.Fail;
            OnCatch(e);
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnPreLoaded() { }
        protected virtual void OnUnloaded() { }
        protected virtual void OnCatch(Exception e) { }
        protected virtual PromiseTask OnLoadAsync() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnPreloadAsync() { return PromiseTask.CompletedTask; }
        protected virtual PromiseTask OnUnloadAsync() { return PromiseTask.CompletedTask; }
    }
}
