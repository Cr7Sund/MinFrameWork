using System;
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

        public virtual void Execute(PromisedT value)
        {
            try
            {
                var newValue = OnExecute(value);
                this.Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    this.ReportProgress(progress);
                }
            }
            catch (Exception e)
            {
                OnCatch(e);
                this.Reject(e);
            }
        }

        public void Progress(float progress)
        {
            OnProgress(progress);
        }

        public IPromiseCommand<PromisedT> Then<T>() where T : IPromiseCommand<PromisedT>, new()
        {
            var resultPromise = new T();

            return this.Then(resultPromise);
        }

        public IPromiseCommand<PromisedT> Then(IPromiseCommand<PromisedT> resultPromise)
        {
            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
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
}