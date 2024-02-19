using System;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class AsyncLoadable : ILoadAsync
    {
        private IPromise _loadGroup;
        private IPromise _unloadGroup;
        private Action _loadedHandler;
        private Action _unloadedHandler;
        private Action<Exception> _exceptionHandler;

        public IPromise LoadStatus
        {
            get
            {
                return _loadGroup;
            }
        }

        public IPromise UnloadStatus
        {
            get
            {
                return _unloadGroup;
            }
        }
        public LoadState State { get; protected set; }


        public AsyncLoadable()
        {
            _unloadedHandler = _Unloaded;
            _loadedHandler = _Loaded;
            _exceptionHandler = _HandleException;
        }

        public IPromise LoadAsync()
        {
            if (State == LoadState.Loading || State == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {State} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            State = LoadState.Loading;

            try
            {
                _loadGroup = OnLoadAsync();
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
            _loadGroup?.Then(onResolved: _loadedHandler, onRejected: _exceptionHandler);

            return _loadGroup;
        }

        public IPromise UnloadAsync()
        {
            if (State == LoadState.Default || State == LoadState.Unloading || State == LoadState.Unloaded)
            {
                throw new MyException($"Cant UnloadAsync On State {State} Loadable: {this} ",
                    NodeTreeExceptionType.UNLOAD_VALID_STATE);
            }
            
            if (State == LoadState.Loading)
            {
                _loadGroup?.Resolve();
            }

            State = LoadState.Unloading;

            try
            {
                _unloadGroup = OnUnloadAsync();
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
            _unloadGroup?.Then(onResolved: _unloadedHandler, onRejected: _exceptionHandler);

            return _unloadGroup;
        }

        private void _Loaded()
        {
            State = LoadState.Loaded;
            OnLoaded();
        }
        private void _Unloaded()
        {
            State = LoadState.Unloaded;
            OnUnloaded();
        }
        private void _HandleException(Exception e)
        {
            State = LoadState.Fail;
            OnCatch(e);
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnUnloaded() { }
        protected virtual void OnCatch(Exception e) { }
        protected abstract IPromise OnLoadAsync();
        protected abstract IPromise OnUnloadAsync();
    }
}
