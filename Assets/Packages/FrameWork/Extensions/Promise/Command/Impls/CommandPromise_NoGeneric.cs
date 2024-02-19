using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Util;
namespace Cr7Sund.PackageTest.Impl
{
    public class CommandPromise : Promise, ICommandPromise
    {

        public IPoolBinder PoolBinder;
        public Action ReleaseHandler;
        public Action<Exception> ErrorHandler;

        private IBaseCommand _command;
        private Action _executeHandler;
        private Action<float> _sequenceProgressHandler;
        private Action<float> _commandProgressHandler;
        private List<IBasePromise> _promisePoolList;


        public float SliceLength { get; set; }
        public int SequenceID { get; set; }
        public bool IsRetain { get; private set; }
        public bool IsOnceOff { get; set; }
        public Action ExecuteHandler
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
            Release();
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
            resultPromise.Reset();
            InitValuePromise(resultPromise);
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

            base.Dispose();
            _command = null;
            ReleasePoolPromises();
        }

        public virtual void Release()
        {
            if (IsOnceOff)
            {
                var pool = PoolBinder?.Get<CommandPromise>();
                pool.ReturnInstance(this);
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
                item.Dispose();
            }
            _promisePoolList.Clear();
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
