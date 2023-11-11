using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework;


namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseCommand : Promise, IPromiseCommand
    {
        public PromiseCommand(PromiseState promiseState) : base(promiseState) { }
        public PromiseCommand() : base() { }

        public virtual void OnCatch(Exception e) { }

        public abstract void OnExecute();

        public virtual void OnProgress(float progress) { }


        #region IPromiseCommand Implementation
        public float SliceLength { get; set; }
        public int SequenceID { get; set; }

        public virtual void Execute()
        {
            try
            {
                OnExecute();
                this.Resolve();

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

        public virtual IPromiseCommand Then<T>() where T : IPromiseCommand, new()
        {
            var resultPromise = new T();

            return this.Then(resultPromise);
        }

        public virtual IPromiseCommand Then(IPromiseCommand resultPromise)
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


    internal class ResolvedPromiseCommand : PromiseCommand
    {
        protected ResolvedPromiseCommand(PromiseState initialState) : base(initialState) { }

        public override void OnExecute() { }


        public new static IPromiseCommand Resolved()
        {
            return resolvedPromise;
        }

        private static IPromiseCommand resolvedPromise = new ResolvedPromiseCommand(PromiseState.Resolved);
    }
}