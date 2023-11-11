using Cr7Sund.Framework.Api;
using NUnit.Framework;
using System;

namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseAsyncCommand<PromisedT> : PromiseCommand<PromisedT>
    {
        public abstract IPromise<PromisedT> OnExecuteAsync(PromisedT value);

        public override PromisedT OnExecute(PromisedT value) => value;

        public override void Execute(PromisedT value)
        {
            try
            {
                var resultPromise = OnExecuteAsync(value);
                Assert.NotNull(resultPromise);
                resultPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * SliceLength ))
                        .Then(
                            (newValue) => this.Resolve(newValue),
                            ex => this.Reject(ex)
                        );
            }
            catch (Exception e)
            {
                OnCatch(e);
                this.Reject(e);
            }
        }
    }
}