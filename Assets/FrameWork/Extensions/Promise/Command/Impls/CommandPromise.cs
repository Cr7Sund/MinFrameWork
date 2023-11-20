using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise<PromisedT> : Promise<PromisedT>, ICommandPromise<PromisedT>
    {
        [Inject] private IPoolBinder _poolBinder;

        private List<ResolveHandler<object>> convertResolveHandlers;
        protected IBaseCommand command;


        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }

        public CommandPromise() : base()
        {
        }

        #region IPromiseCommand Implementation

        public void Execute(PromisedT value)
        {
            ExecuteInternal(value, this);
        }

        protected virtual void ExecuteWarp(object value)
        {
            AssertUtil.IsInstanceOf<PromisedT>(value);
            Execute((PromisedT)value);
        }

        public void Progress(float progress)
        {
            command.OnProgress(progress);
        }

        public virtual void Catch(Exception e)
        {
            if (command is IAsyncCommand<PromisedT> asyncCommand)
            {
                var rejectPromise = asyncCommand.OnCatchAsync(e);
                if (rejectPromise == null)
                {
                    rejectPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * this.SliceLength))
                        .Then(
                            (chainValue) => this.Resolve(chainValue),
                            ex => this.Reject(ex)
                        );

                    return;
                }
            }

            command.OnCatch(e);
        }

        public virtual ICommandPromise<PromisedT> Then<T>() where T : ICommand<PromisedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT>();

            return this.Then(resultPromise, new T());
        }

        public ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, ICommand<PromisedT> command)
        {
            ((CommandPromise<PromisedT>)resultPromise).command = command;

            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            ProgressHandlers(resultPromise, resultPromise.Progress);

            return resultPromise;
        }

        public ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : ICommand<PromisedT, ConvertedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT, ConvertedT>();
            return this.Then(resultPromise, new T());
        }

        public ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise,
            ICommand<PromisedT, ConvertedT> command)
        {
            var specificPromise = (CommandPromise<ConvertedT>)resultPromise;
            specificPromise.command = command;

            AddConvertResolveHandler(specificPromise.ExecuteWarp, resultPromise);
            AddRejectHandler(resultPromise.Reject, resultPromise);
            ProgressHandlers(resultPromise, resultPromise.Progress);
            return resultPromise;
        }

        public ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => All<PromisedT>(promises)) as ICommandPromise<IEnumerable<PromisedT>>;
        }

        public ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var commandPromises = promises as ICommandPromise<PromisedT>[] ?? promises.ToArray();
            FulfillPromise(commandPromises, commands);

            var fns = new Func<IPromise<PromisedT>>[commandPromises.Count()];
            commandPromises.Each((promise, index) => { fns[index] = () => promise; });
            return Then(value => First<PromisedT>(fns)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => Race<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => Any<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        private void FulfillPromise(IEnumerable<ICommandPromise<PromisedT>> promises,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            AssertUtil.AreEqual(commands.Count(), promises.Count());
            var commandArray = commands.ToArray();

            promises.Each((promise, index) => { Then(promise, commandArray[index]); });
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
            if (convertResolveHandlers == null)
            {
                convertResolveHandlers = new List<ResolveHandler<object>>();
            }

            convertResolveHandlers.Add(new ResolveHandler<object>()
            {
                callback = onResolved,
                rejectable = rejectable
            });
        }

        protected override void InvokeResolveHandlers(PromisedT value)
        {
            if (convertResolveHandlers != null)
            {
                for (int i = 0; i < convertResolveHandlers.Count; i++)
                {
                    var handler = convertResolveHandlers[i];
                    InvokeHandler(handler.callback, handler.rejectable, value);
                }
            }

            base.InvokeResolveHandlers(value);
        }

        protected override void ClearHandlers()
        {
            base.ClearHandlers();
            this.convertResolveHandlers = null;
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

            this.CurState = PromiseState.Pending;
            this.resolveValue = default(PromisedT);
            this.Name = string.Empty;

            command = null;
            convertResolveHandlers = null;
        }

        public virtual void Release()
        {
            if (!IsRetain) return;
            command?.Release();
            _poolBinder?.Get<CommandPromise<PromisedT>>().ReturnInstance(this);
        }

        #endregion

        private static void ExecuteInternal(PromisedT value, CommandPromise<PromisedT> promise)
        {
            var command = promise.command;
            if (command is IAsyncCommand<PromisedT> asyncCommand)
            {
                IPromise<PromisedT> resultPromise = null;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    promise.Release();
                    throw e;
                }

                AssertUtil.NotNull(resultPromise);
                resultPromise
                    .Progress(progress => promise.ReportProgress((progress + promise.SequenceID) * promise.SliceLength))
                    .Then(
                        (chainValue) =>
                        {
                            promise.Resolve(chainValue);
                            promise.Release();
                        },
                        ex =>
                        {
                            promise.Reject(ex);
                            promise.Release();
                        }
                    );
            }
            else if (command is ICommand<PromisedT> promiseCommand)
            {
                PromisedT newValue = default(PromisedT);
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    promise.Release();
                    throw e;
                }

                promise.Resolve(newValue);

                float progress = promise.SliceLength * promise.SequenceID;
                if (progress > 0)
                {
                    promise.ReportProgress(progress);
                }

                promise.Release();
            }
        }

        public IBaseCommand Test_GetCommand() => command;
    }


    public class CommandPromise<PromisedT, ConvertedT> : CommandPromise<ConvertedT>
    {
        private static void ExecuteInternal(PromisedT value, CommandPromise<PromisedT, ConvertedT> promise)
        {
            var command = promise.command;

            if (command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
            {
                IPromise<ConvertedT> resultPromise = null;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    promise.Release();
                    throw e;
                }

                AssertUtil.NotNull(resultPromise);
                resultPromise
                    .Progress(progress => promise.ReportProgress((progress + promise.SequenceID) * promise.SliceLength))
                    .Then(
                        (chainValue) =>
                        {
                            promise.Resolve(chainValue);
                            promise.Release();
                        },
                        ex =>
                        {
                            promise.Reject(ex);
                            promise.Release();
                        }
                    );
            }
            else if (command is ICommand<PromisedT, ConvertedT> promiseCommand)
            {
                ConvertedT newValue = default(ConvertedT);
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    promise.Release();
                    throw e;
                }

                promise.Resolve(newValue);

                float progress = promise.SliceLength * promise.SequenceID;
                if (progress > 0)
                {
                    promise.ReportProgress(progress);
                }

                promise.Release();
            }
        }

        protected override void ExecuteWarp(object value)
        {
            AssertUtil.IsInstanceOf<PromisedT>(value);

            ExecuteInternal((PromisedT)value, this);
        }

        public override void Catch(Exception e)
        {
            if (command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
            {
                var rejectPromise = asyncCommand.OnCatchAsync(e);
                if (rejectPromise == null)
                {
                    rejectPromise
                        .Progress(progress => this.ReportProgress((progress + this.SequenceID) * this.SliceLength))
                        .Then(
                            (chainValue) => this.Resolve(chainValue),
                            ex => this.Reject(ex)
                        );

                    return;
                }
            }

            command.OnCatch(e);
        }
    }
}