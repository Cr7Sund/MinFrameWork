using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise<PromisedT> : Promise<PromisedT>, ICommandPromise<PromisedT>
    {
        public IPoolBinder PoolBinder;
        public Action<PromisedT> ReleaseHandler;
        public Action<Exception> ErrorHandler;

        protected IBaseCommand _command;

        private List<ResolveHandler<object>> _convertResolveHandlers;


        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }
        public bool IsOnceOff { get; set; }
        public Action<PromisedT> ExecuteHandler { get; private set; }
        public Action<object> ExecuteWrapHandler { get; private set; }
        public Action<float> SequenceProgressHandler { get; private set; }
        public Action<float> CommandProgressHandler { get; private set; }


        public CommandPromise() : base()
        {
            ExecuteHandler = Execute;
            ExecuteWrapHandler = ExecuteWarp;
            SequenceProgressHandler = SequenceProgress;
            CommandProgressHandler = Progress;
        }

        #region IPromiseCommand Implementation
        public void Execute(PromisedT value)
        {
            ExecuteInternal(value);
        }

        protected virtual void ExecuteWarp(object value)
        {
            AssertUtil.IsInstanceOf<PromisedT>(value);
            Execute((PromisedT)value);
        }

        public void Progress(float progress)
        {
            _command.OnProgress(progress);
        }

        public virtual void Catch(Exception e)
        {
            if (_command is IAsyncCommand<PromisedT> asyncCommand)
            {
                var rejectPromise = asyncCommand.OnCatchAsync(e);
                if (rejectPromise != null)
                {
                    rejectPromise
                        .Progress(SequenceProgressHandler)
                        .Then(ResolveHandler, RejectHandler);

                    return;
                }
            }
            _command.OnCatch(e);
        }

        public virtual ICommandPromise<PromisedT> Then<T>() where T : ICommand<PromisedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT>();

            return Then(resultPromise, new T());
        }

        public ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, ICommand<PromisedT> command)
        {
            ((CommandPromise<PromisedT>)resultPromise)._command = command;

            ActionHandlers(resultPromise, resultPromise.ExecuteHandler, resultPromise.RejectHandler);
            ProgressHandlers(resultPromise, resultPromise.CommandProgressHandler);

            return resultPromise;
        }

        public ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommand<PromisedT, ConvertedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT, ConvertedT>();
            return Then(resultPromise, new T());
        }

        public ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise,
            ICommand<PromisedT, ConvertedT> command)
        {
            var specificPromise = (CommandPromise<ConvertedT>)resultPromise;
            specificPromise._command = command;

            AddConvertResolveHandler(specificPromise.ExecuteWrapHandler, resultPromise);
            AddRejectHandler(resultPromise.RejectHandler, resultPromise);
            ProgressHandlers(resultPromise, resultPromise.ProgressHandler);
            return resultPromise;
        }

        public ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(_ => All<PromisedT>(promises)) as ICommandPromise<IEnumerable<PromisedT>>;
        }

        public ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var commandPromises = promises as ICommandPromise<PromisedT>[] ?? promises.ToArray();
            RegisterPromiseArray(commandPromises, commands);

            var fns = new Func<IPromise<PromisedT>>[commandPromises.Count()];
            commandPromises.Each((promise, index) => { fns[index] = () => promise; });
            return Then(_ => First<PromisedT>(fns)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(_ => Race<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            RegisterPromiseArray(promises, commands);

            return Then(_ => Any<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }


        public IBaseCommand Test_GetCommand()
        {
            return _command;
        }

        protected void SequenceProgress(float progress)
        {
            ReportProgress((progress + SequenceID) * SliceLength);
        }


        private void RegisterPromiseArray(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            AssertUtil.AreEqual(commands.Count(), promises.Count());
            var commandArray = commands.ToArray();

            promises.Each((promise, index) => { Then(promise, commandArray[index]); });
        }

        private void ExecuteInternal(PromisedT value)
        {
            var command = _command;
            if (command is IAsyncCommand<PromisedT> asyncCommand)
            {
                IPromise<PromisedT> resultPromise;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
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
            else if (command is ICommand<PromisedT> promiseCommand)
            {
                PromisedT newValue;
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    Catch(e);

                    throw e;
                }

                Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    ReportProgress(progress);
                }
            }
        }
        #endregion

        #region IPromise Implementation
        public override void Done()
        {
            base.ClearHandlers();
            _convertResolveHandlers?.Clear();
            Then(ReleaseHandler, ErrorHandler);
        }

        public override void Dispose()
        {
            base.Dispose();
            _command = null;
        }

        protected override Promise<T> GetRawPromise<T>()
        {
            return new CommandPromise<T>();
        }

        protected override Promise GetRawPromise()
        {
            return new CommandPromise();
        }

        protected override void ClearHandlers()
        {
            if (IsOnceOff)
            {
                base.ClearHandlers();
                _convertResolveHandlers?.Clear();
            }
        }


        private void AddConvertResolveHandler(Action<object> onResolved, IRejectable rejectable)
        {
            _convertResolveHandlers ??= new List<ResolveHandler<object>>();

            _convertResolveHandlers.Add(new ResolveHandler<object>
            {
                Callback = onResolved,
                Rejectable = rejectable
            });
        }

        protected override void InvokeResolveHandlers(PromisedT value)
        {
            if (_convertResolveHandlers != null)
            {
                for (int i = 0; i < _convertResolveHandlers.Count; i++)
                {
                    var handler = _convertResolveHandlers[i];
                    InvokeHandler(handler.Callback, handler.Rejectable, value);
                }
            }

            base.InvokeResolveHandlers(value);
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
            var pool = PoolBinder?.Get<CommandPromise<PromisedT>>();
            pool?.ReturnInstance(this);
        }
        #endregion

        #region IResetable Implementation
        public void Reset()
        {
            CurState = PromiseState.Pending;
            _resolveValue = default;
        }
        #endregion


    }


    public class CommandPromise<PromisedT, ConvertedT> : CommandPromise<ConvertedT>
    {
        private void ExecuteInternal(PromisedT value)
        {
            var command = _command;

            if (command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
            {
                IPromise<ConvertedT> resultPromise;

                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
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
            else if (command is ICommand<PromisedT, ConvertedT> promiseCommand)
            {
                ConvertedT newValue;
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    Catch(e);

                    throw e;
                }

                Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    ReportProgress(progress);
                }
            }
        }

        protected override void ExecuteWarp(object value)
        {
            AssertUtil.IsInstanceOf<PromisedT>(value);

            ExecuteInternal((PromisedT)value);
        }

        public override void Catch(Exception e)
        {
            if (_command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
            {
                var rejectPromise = asyncCommand.OnCatchAsync(e);
                if (rejectPromise != null)
                {
                    rejectPromise
                        .Progress(SequenceProgressHandler)
                        .Then(ResolveHandler, RejectHandler);
                    return;
                }
            }
            _command.OnCatch(e);
        }
    }
}
