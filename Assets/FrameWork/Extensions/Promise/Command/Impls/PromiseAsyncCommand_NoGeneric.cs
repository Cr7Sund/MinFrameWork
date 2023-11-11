using Cr7Sund.Framework.Api;
using NUnit.Framework;
using System;

namespace Cr7Sund.Framework.Impl
{
    public abstract class PromiseAsyncCommand : PromiseCommand, IPromiseCommand
    {
        public abstract IPromise OnExecuteAsync();

        public override void OnExecute() { }

        public sealed override void Execute()
        {
            try
            {
                var resultPromise = OnExecuteAsync();
                Assert.NotNull(resultPromise);
                resultPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * SliceLength))
                        .Then(
                            () => this.Resolve(),
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