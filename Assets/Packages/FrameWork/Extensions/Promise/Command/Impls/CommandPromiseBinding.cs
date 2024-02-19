using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.PackageTest.Api;
using Cr7Sund.PackageTest.Util;
namespace Cr7Sund.PackageTest.Impl
{
    public class CommandPromiseBinding<PromisedT> : Binding, ICommandPromiseBinding<PromisedT>
    {
        [Inject] private ICommandBinder _commandBinder;
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;
        private ICommandPromise<PromisedT> _firstPromise;
        private List<ICommandPromise<PromisedT>> _promiseList = new List<ICommandPromise<PromisedT>>();
        private Action<PromisedT> _releaseHandler;
        private Action<Exception> _errorHandler;

        public bool IsOnceOff { get; private set; }
        public CommandBindingStatus BindingStatus { get; private set; }
        public List<ICommandPromise<PromisedT>> PromiseList
        {
            get
            {
                return _promiseList ??= new List<ICommandPromise<PromisedT>>();
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

        #region IPromiseCommandBinding<PromisedT> Implementation
        public new ICommandPromiseBinding<PromisedT> To(object value)
        {
            return base.To(value) as ICommandPromiseBinding<PromisedT>;
        }

        public ICommandPromiseBinding<PromisedT> AsOnce()
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
                    ICommandPromise<PromisedT> item = _promiseList[i];
                    item.Reset();
                }
            }

            BindingStatus = CommandBindingStatus.Default;
        }


        public void RunPromise(PromisedT value)
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

            _firstPromise.Resolve(value);
        }

        public override void Dispose()
        {
            ReleasePromise();

            base.Dispose();
            _promiseList?.Clear();
            _firstPromise = null;

            BindingStatus = CommandBindingStatus.Default;
        }

        public List<ICommandPromise<PromisedT>> Test_GetPromiseList()
        {
            return PromiseList;
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.Then<T>()
        {
            var nextPromise = InstantiatePromise();
            var nextCommand = InstantiateCommand<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.Then<T, ConvertedT>()
        {
            var nextPromise = InstantiateConvertPromise<ConvertedT>();
            var nextCommand = InstantiateCommand<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenConvert<T, ConvertedT>()
        {
            var nextPromise = InstantiatePromise<PromisedT, ConvertedT>();
            var nextCommand = InstantiateCommand<T>();

            Then(nextPromise, nextCommand);
            // Assert.NotNull(prevValue, $"the first command try to Convert {typeof(PromisedT)} into {typeof(ConvertedT)}. Currently dont support that. ");

            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenAny(
            params ICommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateArrayPromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenAny<T1, T2>()
        {
            InstantiateArrayPromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenAny<T1, T2, T3>()
        {
            InstantiateArrayPromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace(
            params ICommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateArrayPromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace<T1, T2>()
        {
            InstantiateArrayPromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace<T1, T2, T3>()
        {
            InstantiateArrayPromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst(
            params ICommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateArrayPromise(commands);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst<T1, T2>()
        {
            InstantiateArrayPromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst<T1, T2, T3>()
        {
            InstantiateArrayPromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }
        #endregion

        #region private methods
        private ICommandPromise<PromisedT> ThenRace(ICommandPromise<PromisedT>[] promiseArray,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            return prevValue.ThenRace(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenFirst(ICommandPromise<PromisedT>[] promiseArray,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            return prevValue.ThenFirst(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenAny(ICommandPromise<PromisedT>[] promiseArray,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            return prevValue.ThenAny(promiseArray, commands);
        }

        private void Then(ICommandPromise<PromisedT> nextPromise, ICommand<PromisedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            prevValue.Then(nextPromise, nextCommand);
        }

        private void Then<ConvertedT>(ICommandPromise<ConvertedT> nextPromise, ICommand<ConvertedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<ConvertedT>();
            prevValue.Then(nextPromise, nextCommand);
        }

        private void Then<ConvertedT>(CommandPromise<PromisedT, ConvertedT> nextPromise,
            ICommand<PromisedT, ConvertedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();

            prevValue.Then(nextPromise, nextCommand);
        }

        private ICommandPromise<T> FindPrevChainPromise<T>()
        {
            if (Value.Count > 0)
            {
                AssertUtil.IsInstanceOf<CommandPromise<T>>(Value[^1]);
                var prevValue = (CommandPromise<T>)Value[^1];
                return prevValue;
            }

            AssertUtil.IsTrue(typeof(T) == typeof(PromisedT),
              PromiseExceptionType.CONVERT_FIRST);

            _firstPromise = InstantiatePromise();
            To(_firstPromise);
            return _firstPromise as ICommandPromise<T>;
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

        private ICommandPromise<PromisedT> InstantiatePromise()
        {
            CommandPromise<PromisedT> result;

            if (IsOnceOff)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise<PromisedT>>(CommandPromiseBinding.MaxPoolCount);
                result = pool.GetInstance();
                InitPromise(result);
                result.ReleaseHandler = _releaseHandler;
            }
            else
            {
                result = new CommandPromise<PromisedT>();
                result.ReleaseHandler = _releaseHandler;
                InitPromise(result);
            }

            return result;
        }

        private ICommandPromise<T> InstantiateConvertPromise<T>()
        {
            CommandPromise<T> result;

            if (IsOnceOff)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise<T>>(CommandPromiseBinding.MaxPoolCount);
                result = pool.GetInstance();
                InitPromise<T>(result);
                result.ReleaseHandler = HandleResolve;
            }
            else
            {
                result = new CommandPromise<T>();
                result.ReleaseHandler = HandleResolve;
                InitPromise<T>(result);
            }

            return result;
        }

        private CommandPromise<T1, T2> InstantiatePromise<T1, T2>()
        {
            CommandPromise<T1, T2> result;

            if (IsOnceOff)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise<T1, T2>>(CommandPromiseBinding.MaxPoolCount);
                result = pool.GetInstance();
                InitPromise<T2>(result);
                result.ReleaseHandler = HandleResolve;
            }
            else
            {
                result = new CommandPromise<T1, T2>();
                InitPromise<T2>(result);
                result.ReleaseHandler = HandleResolve;
            }

            return result;
        }

        private void InitPromise<T>(CommandPromise<T> result)
        {
            result.IsOnceOff = IsOnceOff;
            result.PoolBinder = _poolBinder;
            result.ErrorHandler = _errorHandler;
            result.Reset();
        }

        private ICommandPromise<PromisedT>[] InstantiateArrayPromise(ICommand<PromisedT>[] commands)
        {
            var promiseArray = new ICommandPromise<PromisedT>[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }

            return promiseArray;
        }

        private void InstantiateArrayPromise<T1, T2>(out List<ICommand<PromisedT>> commands,
            out ICommandPromise<PromisedT>[] promiseArray)
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
        {
            commands = new List<ICommand<PromisedT>>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>()
            };
            promiseArray = new ICommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }
        }

        private void InstantiateArrayPromise<T1, T2, T3>(out List<ICommand<PromisedT>> commands,
            out ICommandPromise<PromisedT>[] promiseArray)
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
            where T3 : class, ICommand<PromisedT>, new()
        {
            commands = new List<ICommand<PromisedT>>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>(),
                InstantiateCommand<T3>()
            };
            promiseArray = new ICommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
                PromiseList.Add(promiseArray[i]);
            }
        }


        private void HandleResolve<T>(T value)
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
            if (Value != null)
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
                    ICommandPromise<PromisedT> item = _promiseList[i];
                    item.Release();
                }
            }
        }

        #endregion
    }
}
