using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise<PromisedT> : Promise<PromisedT>, ICommandPromise<PromisedT>
    {
        protected IBaseCommand _command;


        private List<ResolveHandler<object>> _convertResolveHandlers;

        public IPoolBinder PoolBinder;

        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }

        private  void ExecuteInternal(PromisedT value)
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
                    Release();
                    throw e;
                }

                AssertUtil.NotNull(resultPromise, new PromiseException("there is an exception happen in OnExecuteAsync ", PromiseExceptionType.EXCEPTION_ON_ExecuteAsync));
                resultPromise
                    .Progress(WrapProgress)
                    .Then(Resolve, Reject);
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
                    Release();
                    throw e;
                }

                Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    ReportProgress(progress);
                }

                Release();
            }
        }

        public IBaseCommand Test_GetCommand()
        {
            return _command;
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
                        .Progress(WrapProgress)
                        .Then(Resolve, Reject);

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

            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            ProgressHandlers(resultPromise, resultPromise.Progress);

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

            AddConvertResolveHandler(specificPromise.ExecuteWarp, resultPromise);
            AddRejectHandler(resultPromise.Reject, resultPromise);
            ProgressHandlers(resultPromise, resultPromise.Progress);
            return resultPromise;
        }

        public ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(_ => All<PromisedT>(promises)) as ICommandPromise<IEnumerable<PromisedT>>;
        }

        public ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var commandPromises = promises as ICommandPromise<PromisedT>[] ?? promises.ToArray();
            FulfillPromise(commandPromises, commands);

            var fns = new Func<IPromise<PromisedT>>[commandPromises.Count()];
            commandPromises.Each((promise, index) => { fns[index] = () => promise; });
            return Then(_ => First<PromisedT>(fns)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(_ => Race<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(_ => Any<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        private void FulfillPromise(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            AssertUtil.AreEqual(commands.Count(), promises.Count());
            var commandArray = commands.ToArray();

            promises.Each((promise, index) => { Then(promise, commandArray[index]); });
        }
        public override void Reject(Exception ex)
        {
            base.Reject(ex);
            Release();
        }

        public override void Resolve(PromisedT value)
        {
            base.Resolve(value);
            Release();
        }
        
        protected void WrapProgress(float progress)
        {
            ReportProgress((progress + SequenceID) * SliceLength);
        }
        
        #endregion

        #region IPromise Implementation
        protected override Promise<T> GetRawPromise<T>()
        {
            return new CommandPromise<T>();
        }

        protected override Promise GetRawPromise()
        {
            return new CommandPromise();
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

        protected override void ClearHandlers()
        {
            base.ClearHandlers();
            _convertResolveHandlers = null;
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
            _resolveValue = default;
            Name = string.Empty;

            _command = null;
            _convertResolveHandlers = null;
        }

        public virtual void Release()
        {
            if (!IsRetain) return;
            PoolBinder?.Get<CommandPromise<PromisedT>>().ReturnInstance(this);
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
                    Release();
                    throw e;
                }

                AssertUtil.NotNull(resultPromise);
                resultPromise
                    .Progress(WrapProgress)
                    .Then(Resolve,Reject);
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
                    Release();
                    throw e;
                }

                Resolve(newValue);

                float progress = SliceLength * SequenceID;
                if (progress > 0)
                {
                    ReportProgress(progress);
                }

                Release();
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
                        .Progress(WrapProgress)
                        .Then(Resolve, Reject);
                    return;
                }
            }
            _command.OnCatch(e);
        }
    }
}
