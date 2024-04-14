using System;
using Cr7Sund.AssetLoader.Api;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Cr7Sund.AssetLoader.Impl
{

    public class AssetPromise : Promise<Object>, IAssetPromise
    {
        public AsyncOperationHandle Handler { get; }
        public string Key { get; }
        public int ControlId { get; }

        public AssetPromise(PromiseState promiseState = PromiseState.Pending) : base(promiseState)
        {

        }

        public AssetPromise(AsyncOperationHandle handler, string key, int controlId)
        {
            Handler = handler;
            Key = key;
            ControlId = controlId;
        }


        public AssetPromise(string key, int controlId)
        {
            Key = key;
            ControlId = controlId;
        }

        public T GetResult<T>() where T : Object
        {
            if (CurState == PromiseState.Pending)// fail
            {
                throw new MyException(AssetLoaderExceptionType.pending_state);
            }
            if (CurState == PromiseState.Rejected)// fail
            {
                throw new MyException(AssetLoaderExceptionType.fail_state);
            }
            if (Key == null)
            {
                return _resolveValue as T;
            }


            if (Handler.IsDone)
            {
                return Handler.Result as T;
            }
            else
            {
                throw new MyException(AssetLoaderExceptionType.no_done_State);
            }
        }
        public async PromiseTask<T> ForceGetResult<T>() where T : Object
        {
            if (CurState == PromiseState.Rejected)
            {
                throw new NotImplementedException();
            }
            if (!Handler.IsDone)
            {
                Handler.WaitForCompletion();
                await ToNewTask();
            }
            return Handler.Result as T;
        }

        protected override Promise<ConvertedT> GetRawPromise<ConvertedT>()
        {
            if (typeof(Object).IsAssignableFrom(typeof(ConvertedT)))
            {
                AssertUtil.NotNull(Handler);

                return new AssetPromise(Handler, Key, ControlId) as Promise<ConvertedT>;
            }

            return base.GetRawPromise<ConvertedT>();
        }

        public IPromise<T> GetConvertPromise<T>() where T : Object
        {
            return Then(asset => Promise<T>.Resolved(asset as T));
        }

        public override void Cancel()
        {
            base.Cancel();
            // Handler.Cancel();
        }
        public override void Dispose()
        {
            base.Dispose();

            if (_resolveValue)
            {
                GameObject.Destroy(_resolveValue);
            }
        }
    }
}