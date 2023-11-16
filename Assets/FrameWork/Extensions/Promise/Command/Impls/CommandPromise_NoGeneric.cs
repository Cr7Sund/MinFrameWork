using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise : Promise, ICommandPromise
    {
        protected IBaseCommand _command;

        public CommandPromise(PromiseState promiseState) : base(promiseState) { }
        public CommandPromise() : base() { }

        #region IPromise Implementation

        protected override Promise<T> GetRawPromise<T>()
        {
            return new CommandPromise<T>();
        }

        protected override Promise GetRawPromise()
        {
            return new CommandPromise();
        }

        #endregion



        #region IPromiseCommand Implementation

        public float SliceLength { get; set; }
        public int SequenceID { get; set; }

        public virtual void Execute()
        {
            if (_command is IPromiseAsyncCommand asyncCommand)
            {
                IPromise resultPromise = null;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync();
                }
                catch (Exception e)
                {
                    this.Catch(e);
                    throw e;
                }

                AssertUtil.NotNull(resultPromise);
                resultPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * this.SliceLength))
                        .Then(
                            () => this.Resolve(),
                            ex => this.Reject(ex)
                        );
            }
            else if (_command is IPromiseCommand command)
            {
                try
                {
                    command.OnExecute();
                }
                catch (Exception e)
                {
                    this.Catch(e);
                    throw e;
                }

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

            return resultPromise;
        }
      
        public ICommandPromise ThenAll(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands)
        {
            FulfillPromise(promises, commands);

            return Then(() => AllInternal(promises)) as ICommandPromise;
        }

        public ICommandPromise ThenRace(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands)
        {
            FulfillPromise(promises, commands);

            return Then(() => RaceInternal(promises)) as ICommandPromise;
        }

        public ICommandPromise ThenAny(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands)
        {
            FulfillPromise(promises, commands);

            return Then(() => AnyInternal(promises)) as ICommandPromise;
        }

        private void FulfillPromise(IEnumerable<ICommandPromise> promises, IEnumerable<IPromiseCommand> commands)
        {
            AssertUtil.AreEqual(commands.Count(), promises.Count());
            var commandArray = commands.ToArray();

            promises.Each((promise, index) =>
            {
                Then(promise, commandArray[index]);
            });
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