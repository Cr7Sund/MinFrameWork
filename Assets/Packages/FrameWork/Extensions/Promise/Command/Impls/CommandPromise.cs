using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Package.Impl
{
    public class CommandPromise<PromisedT> : Promise<PromisedT>, ICommandPromise<PromisedT>
    {
        public IPoolBinder PoolBinder;
        public Action<PromisedT> ReleaseHandler;
        public Action<Exception> ErrorHandler;

        protected IBaseCommand _command;

        private List<ResolveHandler<object>> _convertResolveHandlers;
        public Action<object> _executeWrapHandler;
        private Action<PromisedT> _executeHandler;
        private Action<float> _sequenceProgressHandler;
        private Action<float> _commandProgressHandler;
        private Action<PromisedT> _resolveHandler;
        private Action<Exception> _rejectHandler;
        private Action<float> _progressHandler;

        private List<IBasePromise> _promisePoolList;


        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }
        public bool IsOnceOff { get; set; }
        public Action<object> ExecuteWrapHandler
        {
            get
            {
                if (_executeHandler == null)
                {
                    _executeWrapHandler = ExecuteWarp;
                }
                return _executeWrapHandler;
            }
        }
        public Action<PromisedT> ExecuteHandler
        {
            get
            {
                if (_executeHandler == null)
                {
                    _executeHandler = Execute;
                }
                return _executeHandler;
            }
        }
        public Action<float> SequenceProgressHandler
        {
            get
            {
                if (_sequenceProgressHandler == null)
                {
                    _sequenceProgressHandler = SequenceProgress;
                }
                return _sequenceProgressHandler;
            }
        }
        public Action<float> CommandProgressHandler
        {
            get
            {
                if (_commandProgressHandler == null)
                {
                    _commandProgressHandler = Progress;
                }
                return _commandProgressHandler;
            }
        }
        public Action<PromisedT> ResolveHandler
        {
            get
            {
                if (_resolveHandler == null)
                {
                    _resolveHandler = Resolve;
                }
                return _resolveHandler;
            }
        }
        public Action<Exception> RejectHandler
        {
            get
            {
                if (_rejectHandler == null)
                {
                    _rejectHandler = RejectWithoutDebug;
                }
                return _rejectHandler;
            }
        }
        public Action<float> ProgressHandler
        {
            get
            {
                if (_progressHandler == null)
                {
                    _progressHandler = ReportProgress;
                }
                return _progressHandler;
            }
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

                if (MacroDefine.IsDebug)
                {
                    bool hasMatchingItem = _resolveHandlers != null && _resolveHandlers.Any(item => item.Rejectable == this);
                    AssertUtil.IsFalse(hasMatchingItem);
                }
                AssertUtil.NotNull(resultPromise, PromiseExceptionType.EXCEPTION_ON_ExecuteAsync);
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
            if ((!IsOnceOff && _resolveHandlers == null)
               || IsOnceOff)
            {
                var resultPromise = GetRawPromise();
                ActionHandlers(resultPromise, ReleaseHandler, ErrorHandler);
            }
        }

        public override void Dispose()
        {
            if (_resolveValue is IDisposable disposable
                     && disposable != this)
            {
                disposable.Dispose();
            }
            Name = string.Empty;
            CurState = PromiseState.Pending;
        }

        protected override Promise<T> GetRawPromise<T>()
        {
            CommandPromise<T> resultPromise;
            if (IsOnceOff)
            {
                resultPromise = CreateValuePoolPromise<T>();

                _promisePoolList ??= new List<IBasePromise>();
                _promisePoolList.Add(resultPromise);
            }
            else
            {
                resultPromise = new CommandPromise<T>();
                InitValuePromise(resultPromise);
            }
            return resultPromise;
        }

        protected override Promise GetRawPromise()
        {
            CommandPromise resultPromise;
            if (IsOnceOff)
            {
                resultPromise = CreateNoValuePromise();
                _promisePoolList ??= new List<IBasePromise>();
                _promisePoolList.Add(resultPromise);
            }
            else
            {
                resultPromise = new CommandPromise();
                InitNoValuePromise(resultPromise);
            }
            return resultPromise;
        }

        private CommandPromise CreateNoValuePromise()
        {
            IPool<CommandPromise> pool = PoolBinder.GetOrCreate<CommandPromise>(CommandPromiseBinding.MaxPoolCount);
            CommandPromise resultPromise = pool.GetInstance();
            resultPromise.Reset();
            InitNoValuePromise(resultPromise);
            return resultPromise;
        }

        private CommandPromise<T> CreateValuePoolPromise<T>()
        {
            CommandPromise<T> resultPromise = PoolBinder.GetOrCreate<CommandPromise<T>>(CommandPromiseBinding.MaxPoolCount).GetInstance();
            InitValuePromise(resultPromise);
            resultPromise.Reset();
            return resultPromise;
        }

        private void InitValuePromise<T>(CommandPromise<T> resultPromise)
        {
            resultPromise.IsOnceOff = IsOnceOff;
            resultPromise.PoolBinder = PoolBinder;
        }

        private void InitNoValuePromise(CommandPromise resultPromise)
        {
            resultPromise.IsOnceOff = IsOnceOff;
            resultPromise.PoolBinder = PoolBinder;
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
            if (IsOnceOff)
            {
                base.ClearHandlers();
                _convertResolveHandlers?.Clear();
            }
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

            Dispose();
            _command = null;
            ReleasePoolPromises();
        }

        public virtual void Release()
        {
            if (IsOnceOff)
            {
                var pool = PoolBinder.Get<CommandPromise<PromisedT>>();
                pool?.ReturnInstance(this);
            }
            else
            {
                ReleasePoolPromises();
            }
        }

        private void ReleasePoolPromises()
        {
            if (_promisePoolList == null) return;
            for (int i = 0; i < _promisePoolList.Count; i++)
            {
                IBasePromise item = _promisePoolList[i];
                if (item is IPoolable poolable)
                {
                    poolable.Release();
                }
                item.Dispose();
            }
            _promisePoolList.Clear();
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

                if (MacroDefine.IsDebug)
                {
                    bool hasMatchingItem = _resolveHandlers != null && _resolveHandlers.Any(item => item.Rejectable == this);
                    AssertUtil.IsFalse(hasMatchingItem);
                }
                AssertUtil.NotNull(resultPromise, PromiseExceptionType.EXCEPTION_ON_ExecuteAsync);

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
