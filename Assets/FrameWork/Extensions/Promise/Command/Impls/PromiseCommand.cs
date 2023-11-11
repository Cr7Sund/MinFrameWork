using System;
using System.Diagnostics;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework;


namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseCommand<PromisedT> : Promise<PromisedT>, IPromiseCommand<PromisedT>
    {
        public PromiseCommand() : base() { }
        protected PromiseCommand(PromiseState initialState) : base(initialState) { }

        public virtual void OnCatch(Exception e) { }

        public abstract PromisedT OnExecute(PromisedT value);

        public virtual void OnProgress(float progress) { }


        #region IPromiseCommand Implementation

        public float SliceLength { get; set; }
        public int SequenceID { get; set; }

        protected virtual void Execute(PromisedT value)
        {
            var newValue = OnExecute(value);
            this.Resolve(newValue);

            float progress = SliceLength * SequenceID;
            if (progress > 0)
            {
                this.ReportProgress(progress);
            }
        }

        public virtual void Execute(object value)
        {
            Assert.IsInstanceOf<PromisedT>(value);

            this.Execute((PromisedT)value);
        }

        public void Progress(float progress)
        {
            OnProgress(progress);
        }

        public void Catch(Exception e)
        {
            UnityEngine.Debug.Log(e);
            OnCatch(e);
        }

        public IPromiseCommand<PromisedT> Then<T>() where T : IPromiseCommand<PromisedT>, new()
        {
            var resultPromise = new T();

            return this.Then(resultPromise);
        }

        public IPromiseCommand<ConvertedT> Then<T, ConvertedT>() where T : IPromiseCommand<ConvertedT>, new()
        {
            var resultPromise = new T();

            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            resultPromise.Catch(resultPromise.Catch);

            ProgressHandlers(resultPromise, resultPromise.Progress);
            return resultPromise;
        }

        public IPromiseCommand<PromisedT> Then(IPromiseCommand<PromisedT> resultPromise)
        {
            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            resultPromise.Catch(resultPromise.Catch);

            ProgressHandlers(resultPromise, resultPromise.Progress);
            return resultPromise;
        }

        #endregion

        #region IPoolable Implementation

        public bool IsRetain { get; private set; }

        public void Retain()
        {
            IsRetain = true;
        }

        public void Restore()
        {
            IsRetain = false;
        }

        public virtual void Release() { }
        #endregion

    }

    internal class ResolvedPromiseCommand<PromisedT> : PromiseCommand<PromisedT>
    {
        protected ResolvedPromiseCommand(PromiseState initialState) : base(initialState) { }

        public override PromisedT OnExecute(PromisedT value) => value;


        public new static IPromiseCommand<PromisedT> Resolved(PromisedT promisedValue)
        {
            var promise = resolvedPromise as ResolvedPromiseCommand<PromisedT>;
            promise.resolveValue = promisedValue;
            return promise;
        }

        private static IPromiseCommand<PromisedT> resolvedPromise = new ResolvedPromiseCommand<PromisedT>(PromiseState.Resolved);
    }

    public abstract class ConvertCommand<PromisedT> : PromiseCommand<PromisedT>
    {
        public override PromisedT OnExecute(PromisedT value) { throw new System.NotImplementedException(); }
        public abstract object OnConvert(PromisedT value);

        protected sealed override void Execute(PromisedT value) { }
        public sealed override void Execute(object value)
        {
            try
            {
                var newValue = OnConvert((PromisedT)value);
                this.Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    this.ReportProgress(progress);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                OnCatch(e);
                this.Reject(e);
            }
        }
    }
}