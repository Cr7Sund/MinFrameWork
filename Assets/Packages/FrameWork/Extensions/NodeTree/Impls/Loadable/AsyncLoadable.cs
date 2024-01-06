using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class AsyncLoadable<T> : ILoadAsync<T>
    {
        private IPromise<T> _loadGroup;
        private IPromise<T> _unloadGroup;
        private Action<T> _loadedHandler;
        private Action<T> _unloadedHandler;
        private Action<Exception> _exceptionHandler;

        public IPromise<T> LoadStatus
        {
            get
            {
                return _loadGroup;
            }
        }

        public IPromise<T> UnloadStatus
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

        public IPromise<T> LoadAsync(T value)
        {
            if (State == LoadState.Loading || State == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {State} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            State = LoadState.Loading;

            try
            {
                _loadGroup = OnLoadAsync(value);
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
            _loadGroup?.Then(onResolved: _loadedHandler, onRejected: _exceptionHandler);

            return _loadGroup;
        }

        public IPromise<T> UnloadAsync(T value)
        {
            if (State == LoadState.Default || State == LoadState.Unloading || State == LoadState.Unloaded)
            {
                throw new MyException($"Cant UnloadAsync On State: {State}  Loadable: {this} ",
                    NodeTreeExceptionType.UNLOAD_VALID_STATE);
            }

            if (State == LoadState.Loading)
            {
                _unloadGroup?.Resolve(value);
            }

            State = LoadState.Unloading;

            try
            {
                _unloadGroup = OnUnloadAsync(value);
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
            _unloadGroup?.Then(onResolved: _unloadedHandler, onRejected: _exceptionHandler);

            return _unloadGroup;
        }

        private void _Loaded(T value)
        {
            State = LoadState.Loaded;
            OnLoaded();
        }
        private void _Unloaded(T value)
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
        protected abstract IPromise<T> OnLoadAsync(T content);
        protected abstract IPromise<T> OnUnloadAsync(T content);
    }
}
