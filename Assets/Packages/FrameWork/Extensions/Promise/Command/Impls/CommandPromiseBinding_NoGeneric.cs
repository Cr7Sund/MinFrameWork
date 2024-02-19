using System.Collections.Generic;
using System.Linq;
using System;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Util;
namespace Cr7Sund.PackageTest.Impl
{
    public class CommandPromiseBinding : Binding, ICommandPromiseBinding
    {
        public const int MaxPoolCount = 16;

        [Inject] private ICommandBinder _commandBinder;
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;
        private List<ICommandPromise> _promiseList;
        private ICommandPromise _firstPromise;
        private Action _releaseHandler;
        private Action<Exception> _errorHandler;

        public bool IsOnceOff { get; private set; }
        public CommandBindingStatus BindingStatus { get; private set; }
        public List<ICommandPromise> PromiseList
        {
            get
            {
                if (_promiseList == null)
                {
                    _promiseList = new List<ICommandPromise>();
                }
                return _promiseList;
            }
        }

        public CommandPromiseBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            _releaseHandler = HandleResolve;
            _errorHandler = HandleRejected;

            ValueConstraint = BindingConstraintType.MANY;
            KeyConstraint = BindingConstraintType.ONE;
            _value.InflationType = PoolInflationType.DOUBLE;
        }

        public CommandPromiseBinding() : this(null)
        {

        }


        #region IPromiseCommandBinding Implementation
        public ICommandPromiseBinding AsOnce()
        {
            IsOnceOff = true;
            return this;
        }

        public void RestartPromise()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                object item = Value[i];
                var poolable = item as IResetable;
                poolable.Reset();
            }

            if (_promiseList != null)
            {
                for (int i = 0; i < _promiseList.Count; i++)
                {
                    ICommandPromise item = _promiseList[i];
                    item.Reset();
                }
            }

            BindingStatus = CommandBindingStatus.Default;
        }

        public void RunPromise()
        {
            AssertUtil.Greater(Value.Count, 0,
                 PromiseExceptionType.EMPTY_PROMISE_TOREACT);

            float sliceLength = 1 / Value.Count;
            for (int i = 0; i < Value.Count; i++)
            {
                var command = Value[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            BindingStatus = CommandBindingStatus.Running;


            var lastPromise = Value[^1] as IBasePromise;
            lastPromise.Done();

            _firstPromise.Resolve();
        }

        public override void Dispose()
        {
            ReleasePromise();

            base.Dispose();
            _promiseList?.Clear();
            _firstPromise = null;

            BindingStatus = CommandBindingStatus.Default;
        }

        private new ICommandPromiseBinding To(object value)
        {
            return base.To(value) as ICommandPromiseBinding;
        }

        ICommandPromiseBinding ICommandPromiseBinding.Then<T>()
        {
            var nextPromise = InstantiatePromise();
            var nextCommand = InstantiateCommand<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenAny(params ICommand[] commands)
        {
            ICommandPromise[] promiseArray = InstantiateArrayPromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenAny<T1, T2>()
        {
            InstantiateArrayPromise<T1, T2>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenAny<T1, T2, T3>()
        {
            InstantiateArrayPromise<T1, T2, T3>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace(params ICommand[] commands)
        {
            var promiseArray = InstantiateArrayPromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace<T1, T2>()
        {
            InstantiateArrayPromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace<T1, T2, T3>()
        {
            InstantiateArrayPromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }
        #endregion

        #region private methods
        private ICommandPromise ThenRace(ICommandPromise[] promiseArray, IEnumerable<ICommand> commands)
        {
            ICommandPromise prevValue = FindPrevChainPromise();
            return prevValue.ThenRace(promiseArray, commands);
        }


        private ICommandPromise ThenAny(ICommandPromise[] promiseArray, IEnumerable<ICommand> commands)
        {
            ICommandPromise prevValue = FindPrevChainPromise();

            return prevValue.ThenAny(promiseArray, commands);
        }

        private void Then(ICommandPromise nextPromise, ICommand nextCommand)
        {
            ICommandPromise prevValue = FindPrevChainPromise();

            prevValue.Then(nextPromise, nextCommand);
        }


        private ICommandPromise FindPrevChainPromise()
        {
            if (Value.Count > 0)
            {
                var prevValue = (ICommandPromise)Value[^1];
                return prevValue;
            }

            _firstPromise = InstantiatePromise();
            To(_firstPromise);
            return _firstPromise;
        }


        private T InstantiateCommand<T>() where T : class, IBaseCommand
        {
            var result = _commandBinder.Get<T>();
            if (result == null)
            {
                result = _commandBinder.GetOrCreate<T>();
                _injectionBinder.Injector.Inject(result);
            }

            return result;
        }

        private CommandPromise InstantiatePromise()
        {
            CommandPromise result;

            if (IsOnceOff)
            {
                IPool<CommandPromise> pool = _poolBinder.GetOrCreate<CommandPromise>(CommandPromiseBinding.MaxPoolCount);
                result = pool.GetInstance();
                InitPromise(result);
            }
            else
            {
                result = new CommandPromise();
                InitPromise(result);
            }

            return result;
        }

        private void InitPromise(CommandPromise result)
        {
            result.IsOnceOff = IsOnceOff;
            result.ReleaseHandler = _releaseHandler;
            result.ErrorHandler = _errorHandler;
            result.PoolBinder = _poolBinder;
            result.Reset();
        }

        private void InstantiateArrayPromise<T1, T2>(out List<ICommand> commands, out ICommandPromise[] promiseArray)
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
        {
            commands = new List<ICommand>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>()
            };
            promiseArray = new ICommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }
        }

        private void InstantiateArrayPromise<T1, T2, T3>(out List<ICommand> commands, out ICommandPromise[] promiseArray)
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new()
        {
            commands = new List<ICommand>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>(),
                InstantiateCommand<T3>()
            };
            promiseArray = new ICommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }
        }

        private ICommandPromise[] InstantiateArrayPromise(ICommand[] commands)
        {
            var promiseArray = new ICommandPromise[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }

            return promiseArray;
        }

        private void HandleResolve()
        {
            BindingStatus = CommandBindingStatus.Default;
            ResolveRelease();
        }

        private void HandleRejected(Exception e)
        {
            BindingStatus = CommandBindingStatus.Default;
            ResolveRelease();
            Debug.Error(e);
        }

        private void ResolveRelease()
        {
            if (IsOnceOff)
            {
                Dispose();
                BindingStatus = CommandBindingStatus.Released;
            }
            else
            {
                ReleasePromise();
            }
        }

        private void ReleasePromise()
        {
            if (Value.Count > 0)
            {
                for (int i = 0; i < Value.Count; i++)
                {
                    object item = Value[i];
                    var poolable = item as IPoolable;
                    poolable.Release();
                }
            }

            if (_promiseList != null)
            {
                for (int i = 0; i < _promiseList.Count; i++)
                {
                    ICommandPromise item = _promiseList[i];
                    item.Release();
                }
            }
        }
        #endregion
    }

}
