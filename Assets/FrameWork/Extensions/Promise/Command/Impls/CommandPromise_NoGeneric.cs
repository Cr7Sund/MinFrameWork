using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise : Promise, ICommandPromise
    {
        protected IBaseCommand _command;

        public CommandPromise(PromiseState promiseState) : base(promiseState) { }
        public CommandPromise() : base() { }



        #region IPromiseCommand Implementation

        public float SliceLength { get; set; }
        public int SequenceID { get; set; }

        public virtual void Execute()
        {
            if (_command is IPromiseAsyncCommand asyncCommand)
            {
                var resultPromise = asyncCommand.OnExecuteAsync();
                Assert.NotNull(resultPromise);
                resultPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * this.SliceLength))
                        .Then(
                            () => this.Resolve(),
                            ex => this.Reject(ex)
                        );
            }
            else if (_command is IPromiseCommand command)
            {
                command.OnExecute();
                this.Resolve();

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    this.ReportProgress(progress);
                }
            }

        }

        public void Catch(Exception e)
        {
            _command.OnCatch(e);
        }
        public void Progress(float progress)
        {
            _command.OnProgress(progress);
        }

        public virtual ICommandPromise Then<T>() where T : IPromiseCommand, new()
        {
            var resultPromise = new CommandPromise();

            return this.Then(resultPromise, new T());
        }

        public virtual ICommandPromise Then(ICommandPromise resultPromise, IPromiseCommand promiseCommand)
        {
            ((CommandPromise)resultPromise)._command = promiseCommand;
            
            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            ProgressHandlers(resultPromise, resultPromise.Progress);
            resultPromise.Catch(resultPromise.Catch);

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
}