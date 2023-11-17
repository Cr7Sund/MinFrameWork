using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromise<PromisedT> : Promise<PromisedT>, ICommandPromise<PromisedT>
    {
        protected IBaseCommand _command;

        public CommandPromise() : base() { }
        protected CommandPromise(PromiseState initialState) : base(initialState) { }


        #region IPromiseCommand Implementation

        public float SliceLength { get; set; }
        public int SequenceID { get; set; }

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
            _command.OnProgress(progress);
        }

        public virtual void Catch(Exception e)
        {
            if (_command is IPromiseAsyncCommand<PromisedT> asyncCommand)
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

            _command.OnCatch(e);
        }

        public virtual ICommandPromise<PromisedT> Then<T>() where T : IPromiseCommand<PromisedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT>();

            return this.Then(resultPromise, new T());
        }

        public ICommandPromise<PromisedT> Then(ICommandPromise<PromisedT> resultPromise, IPromiseCommand<PromisedT> promiseCommand)
        {
            ((CommandPromise<PromisedT>)resultPromise)._command = promiseCommand;

            ActionHandlers(resultPromise, resultPromise.Execute, resultPromise.Reject);
            ProgressHandlers(resultPromise, resultPromise.Progress);
            
            return resultPromise;
        }

        public ICommandPromise<ConvertedT> Then<T, ConvertedT>() where T : IPromiseCommand<PromisedT, ConvertedT>, new()
        {
            var resultPromise = new CommandPromise<PromisedT, ConvertedT>();
            return this.Then(resultPromise, new T());
        }

        public ICommandPromise<ConvertedT> Then<ConvertedT>(ICommandPromise<ConvertedT> resultPromise, IPromiseCommand<PromisedT, ConvertedT> promiseCommand)
        {
            var specificPromise = (CommandPromise<ConvertedT>)resultPromise;
            specificPromise._command = promiseCommand;

            AddConvertResolveHandler(specificPromise.ExecuteWarp, resultPromise);
            AddRejectHandler(resultPromise.Reject, resultPromise);
            ProgressHandlers(resultPromise, resultPromise.Progress);
            return resultPromise;
        }

        public ICommandPromise<IEnumerable<PromisedT>> ThenAll(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => All<PromisedT>(promises)) as ICommandPromise<IEnumerable<PromisedT>>;
        }

        public ICommandPromise<PromisedT> ThenFirst(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            var fns = new Func<IPromise<PromisedT>>[promises.Count()];
            promises.Each((promise, index) =>
            {
                fns[index] = () => promise;
            });
            return Then(value => First<PromisedT>(fns)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenRace(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => Race<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        public ICommandPromise<PromisedT> ThenAny(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            FulfillPromise(promises, commands);

            return Then(value => Any<PromisedT>(promises)) as ICommandPromise<PromisedT>;
        }

        private void FulfillPromise(IEnumerable<ICommandPromise<PromisedT>> promises, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            AssertUtil.AreEqual(commands.Count(), promises.Count());
            var commandArray = commands.ToArray();

            promises.Each((promise, index) =>
            {
                Then(promise, commandArray[index]);
            });
        }


        #endregion

        #region IPromise Implementation

        private List<ResolveHandler<object>> convertResolveHandlers;
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

        private static void ExecuteInternal(PromisedT value, CommandPromise<PromisedT> promise)
        {
            var command = promise._command;
            if (command is IPromiseAsyncCommand<PromisedT> asyncCommand)
            {
                IPromise<PromisedT> resultPromise = null;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    throw e;
                }

                AssertUtil.NotNull(resultPromise);
                resultPromise
                        .Progress(progress => promise.ReportProgress((progress + promise.SequenceID) * promise.SliceLength))
                        .Then(
                            (chainValue) => promise.Resolve(chainValue),
                            ex => promise.Reject(ex)
                        );
            }
            else if (command is IPromiseCommand<PromisedT> promiseCommand)
            {
                PromisedT newValue = default(PromisedT);
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    throw e;
                }
                promise.Resolve(newValue);

                float progress = promise.SliceLength * promise.SequenceID;
                if (progress > 0)
                {
                    promise.ReportProgress(progress);
                }
            }
        }

    }


    public class CommandPromise<PromisedT, ConvertedT> : CommandPromise<ConvertedT>
    {
        private static void ExecuteInternal(PromisedT value, CommandPromise<PromisedT, ConvertedT> promise)
        {
            var _command = promise._command;

            if (_command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
            {
                IPromise<ConvertedT> resultPromise = null;
                try
                {
                    resultPromise = asyncCommand.OnExecuteAsync(value);
                    AssertUtil.NotNull(resultPromise);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    throw e;
                }
                resultPromise
                        .Progress(progress => promise.ReportProgress((progress + promise.SequenceID) * promise.SliceLength))
                        .Then(
                            (chainValue) => promise.Resolve(chainValue),
                            ex => promise.Reject(ex)
                        );
            }
            else if (_command is IPromiseCommand<PromisedT, ConvertedT> promiseCommand)
            {
                ConvertedT newValue = default(ConvertedT);
                try
                {
                    newValue = promiseCommand.OnExecute(value);
                }
                catch (Exception e)
                {
                    promise.Catch(e);
                    throw e;
                }
                promise.Resolve(newValue);

                float progress = promise.SliceLength * promise.SequenceID;
                if (progress > 0)
                {
                    promise.ReportProgress(progress);
                }
            }
        }

        protected override void ExecuteWarp(object value)
        {
            AssertUtil.IsInstanceOf<PromisedT>(value);

            ExecuteInternal((PromisedT)value, this);
        }

        public override void Catch(Exception e)
        {
            if (_command is IPromiseAsyncCommand<PromisedT, ConvertedT> asyncCommand)
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

            _command.OnCatch(e);
        }

    }
}