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
        private Action<T> _preloadedHandler;
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
        public LoadState LoadState { get; protected set; }


        public AsyncLoadable()
        {
            _unloadedHandler = _Unloaded;
            _loadedHandler = _Loaded;
            _preloadedHandler = _Preloaded;
            _exceptionHandler = _HandleException;
        }
        public IPromise<T> LoadAsync(T value)
        {
            if (LoadState == LoadState.Loading || LoadState == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            LoadState = LoadState.Loading;
            _loadGroup = OnLoadAsync(value);
            _loadGroup?.Then(onResolved: _loadedHandler, onRejected: _exceptionHandler);

            return _loadGroup;
        }
        public IPromise<T> PreLoadAsync(T value)
        {
            if (LoadState == LoadState.Loading || LoadState == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {LoadState} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }

            LoadState = LoadState.Loading;
            _loadGroup = OnLoadAsync(value);
            _loadGroup?.Then(onResolved: _preloadedHandler, onRejected: _exceptionHandler);

            return _loadGroup;
        }
        public IPromise<T> UnloadAsync(T value)
        {
            if (LoadState == LoadState.Default || LoadState == LoadState.Unloading || LoadState == LoadState.Unloaded)
            {
                throw new MyException($"Cant UnloadAsync On State: {LoadState}  Loadable: {this} ",
                    NodeTreeExceptionType.UNLOAD_VALID_STATE);
            }

            if (LoadState == LoadState.Loading)
            {
                _unloadGroup?.Resolve(value);
            }

            LoadState = LoadState.Unloading;
            _unloadGroup = OnUnloadAsync(value);
            _unloadGroup?.Then(onResolved: _unloadedHandler, onRejected: _exceptionHandler);

            return _unloadGroup;
        }
        public IPromise<T> CancelLoad()
        {
            AssertUtil.NotNull(LoadStatus, NodeTreeExceptionType.CANCEL_NOTLOADED);

            LoadStatus.Cancel();
            return LoadStatus.Then(UnloadAsync);
        }
        private void _Loaded(T value)
        {
            LoadState = LoadState.Loaded;
            OnLoaded();
        }
        private void _Preloaded(T value)
        {
            LoadState = LoadState.Loaded;
            OnPreLoaded();
        }
        private void _Unloaded(T value)
        {
            LoadState = LoadState.Unloaded;
            OnUnloaded();
        }
        private void _HandleException(Exception e)
        {
            LoadState = LoadState.Fail;
            OnCatch(e);
            Debug.Error(e);
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnPreLoaded() { }
        protected virtual void OnUnloaded() { }
        protected virtual void OnCatch(Exception e) { }
        protected abstract IPromise<T> OnLoadAsync(T content);
        protected abstract IPromise<T> OnUnloadAsync(T content);
    }
}
