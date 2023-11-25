using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise : Promise, ICommandPromise
    {

        public IPoolBinder PoolBinder;
        public Action ReleaseHandler;
        public Action<Exception> ErrorHandler;

        private IBaseCommand _command;


        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }
        public bool IsOnceOff { get; set; }
        public Action ExecuteHandler { get; private set; }
        public Action<float> SequenceProgressHandler { get; private set; }
        public Action<float> CommandProgressHandler { get; private set; }



        public CommandPromise() : base()
        {
            ExecuteHandler = Execute;
            SequenceProgressHandler = SequenceProgress;
            CommandProgressHandler = Progress;
        }

        #region IPromise Implementation
        public override void Done()
        {
            base.ClearHandlers();
            Then(ReleaseHandler, ErrorHandler);
        }

        public override void Dispose()
        {
            base.Dispose();
            _command = null;
        }

        protected override void ClearHandlers()
        {
            if (IsOnceOff)
            {
                base.ClearHandlers();
            }
        }

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
        public virtual void Execute()
        {
            if (_command is IAsyncCommand asyncCommand)
            {
                IPromise resultPromise;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync();
                }
                catch (Exception e)
                {
                    Catch(e);

                    throw e;
                }

                bool hasMatchingItem = _resolveHandlers != null && _resolveHandlers.Any(item => item.Rejectable == this);
                AssertUtil.IsFalse(hasMatchingItem);
                AssertUtil.NotNull(resultPromise, new PromiseException("there is an exception happen in OnExecuteAsync ", PromiseExceptionType.EXCEPTION_ON_ExecuteAsync));

                resultPromise
                    .Progress(SequenceProgressHandler)
                    .Then(ResolveHandler, RejectHandler);
            }
            else if (_command is ICommand command)
            {
                try
                {
                    command.OnExecute();
                }
                catch (Exception e)
                {
                    Catch(e);

                    throw e;
                }

                Resolve();

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    ReportProgress(progress);
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

        public virtual ICommandPromise Then<T>() where T : ICommand, new()
        {
            var resultPromise = new CommandPromise();

            return Then(resultPromise, new T());
        }

        public virtual ICommandPromise Then(ICommandPromise resultPromise, ICommand command)
        {
            ((CommandPromise)resultPromise)._command = command;

            ActionHandlers(resultPromise, resultPromise.ExecuteHandler, resultPromise.RejectHandler);
            ProgressHandlers(resultPromise, resultPromise.CommandProgressHandler);

            return resultPromise;
        }

        public ICommandPromise ThenAll(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(() => AllInternal(promises)) as ICommandPromise;
        }

        public ICommandPromise ThenRace(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(() => RaceInternal(promises)) as ICommandPromise;
        }

        public ICommandPromise ThenAny(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(() => AnyInternal(promises)) as ICommandPromise;
        }

        public IBaseCommand Test_GetCommand()
        {
            return _command;
        }

        private void SequenceProgress(float progress)
        {
            ReportProgress((progress + SequenceID) * SliceLength);
        }

        private void RegisterPromiseArray(IEnumerable<ICommandPromise> promises, IEnumerable<ICommand> commands)
        {
            var commandPromises = promises as ICommandPromise[] ?? promises.ToArray();
            var commandArray = commands.ToArray();
            AssertUtil.AreEqual(commandPromises.Length, commandArray.Length);

            commandPromises.Each((promise, index) =>
            {
                Then(promise, commandArray[index]);
            });
        }
        #endregion

        #region IPoolable Implementation
        public void Retain()
        {
            IsRetain = true;
        }

        public void Restore()
        {
            IsRetain = false;

            CurState = PromiseState.Pending;

            Dispose();
        }

        public virtual void Release()
        {
            var pool = PoolBinder?.Get<CommandPromise>();
            pool?.ReturnInstance(this);
        }
        #endregion

        #region IResetable Implementation
        public void Reset()
        {
            CurState = PromiseState.Pending;

        }
        #endregion
    }
}
