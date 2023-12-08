using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using Cr7Sund.NodeTree.Api;
namespace Cr7Sund.NodeTree.Impl
{
    public abstract class AsyncLoadable : ILoadAsync
    {
        private IPromise _loadGroup;
        private IPromise _unloadGroup;
        private Action _loadHandler;
        private Action _unloadedHandler;
        private Action<Exception> _exceptioHandler;

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
            _loadHandler = _Loaded;
            _exceptioHandler = _HandleException;
        }

        public IPromise LoadAsync()
        {
            if (State == LoadState.Loading || State == LoadState.Unloading)
            {
                throw new MyException($"Cant LoadAsync On State {State} Loadable: {this} ",
                    NodeTreeExceptionType.LOAD_VALID_STATE);
            }
            
            State = LoadState.Loading;

            _loadGroup = OnLoadAsync();
            _loadGroup?.Then(onResolved: _loadHandler, onRejected: _exceptioHandler);

            return _loadGroup;
        }

        public IPromise UnloadAsync()
        {
            AssertUtil.IsFalse(State != LoadState.Loading && State != LoadState.Loaded, $"Connot UnloadAsync On State {State}  Loadable: {this} ");

            if (State == LoadState.Loading)
            {
                _loadGroup?.Resolve();
            }

            State = LoadState.Unloading;

            _unloadGroup = OnUnloadAsync();
            _unloadGroup?.Then(onResolved: _unloadedHandler, onRejected: _exceptioHandler);

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

        }

        protected virtual void OnLoaded() { }
        protected virtual void OnUnloaded() { }
        protected abstract IPromise OnLoadAsync();
        protected abstract IPromise OnUnloadAsync();
    }
}
