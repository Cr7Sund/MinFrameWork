using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinding<PromisedT> : Binding, ICommandPromiseBinding<PromisedT>
    {
        [Inject] private ICommandBinder _commandBinder;
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;
        private ICommandPromise<PromisedT> _firstPromise;
        private List<ICommandPromise<PromisedT>> _promiseList = new List<ICommandPromise<PromisedT>>();

        public bool IsOnceOff { get; private set; }
        public CommandBindingStatus BindingStatus { get; private set; }

        public CommandPromiseBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            ValueConstraint = BindingConstraintType.MANY;
            KeyConstraint = BindingConstraintType.ONE;
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
            if (IsOnceOff) return;

            var values = Value as object[];
            foreach (var item in values)
            {
                var poolable = item as IResetable;
                poolable.Reset();
            }

            foreach (var item in _promiseList)
            {
                item.Reset();
            }

            BindingStatus = CommandBindingStatus.Default;
        }


        public void RunPromise(PromisedT value)
        {
            object[] values = Value as object[];
            AssertUtil.Greater(values.Length, 0, new PromiseException(
                "can not react a empty promise command", PromiseExceptionType.EMPTY_PROMISE_TOREACT));

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            BindingStatus = CommandBindingStatus.Running;

            var lastPromise = values[^1] as IBasePromise;
            lastPromise.Done();

            _firstPromise.Resolve(value);
        }

        public override void Dispose()
        {
            base.Dispose();
            _promiseList.Clear();
            _firstPromise?.Dispose();
            _firstPromise = null;
        }

        public List<ICommandPromise<PromisedT>> Test_GetPromiseList()
        {
            return _promiseList;
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.Then<T>()
        {
            var nextPromise = InstantiatePromise<PromisedT>();
            var nextCommand = InstantiateCommand<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.Then<T, ConvertedT>()
        {
            var nextPromise = InstantiatePromise<ConvertedT>();
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
            if (Value is object[] values)
            {
                AssertUtil.IsInstanceOf<CommandPromise<T>>(values[^1]);
                var prevValue = (CommandPromise<T>)values[^1];
                return prevValue;
            }

            AssertUtil.IsTrue(typeof(T) == typeof(PromisedT), new PromiseException
                ("can convert type in first promise", PromiseExceptionType.CONVERT_FIRST));

            _firstPromise = InstantiatePromise<PromisedT>();
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

        private ICommandPromise<T> InstantiatePromise<T>()
        {
            CommandPromise<T> result;

            if (IsOnceOff)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise<T>>();
                result = pool.GetInstance();
                result.PoolBinder = _poolBinder;
                result.IsOnceOff = IsOnceOff;
                result.ReleaseHandler = HandleResolve;
                result.ErrorHandler = HandleRejected;
            }
            else
            {
                result = new CommandPromise<T>();
                result.IsOnceOff = IsOnceOff;
                result.ReleaseHandler = HandleResolve;
                result.ErrorHandler = HandleRejected;
            }


            return result;
        }

        private CommandPromise<T1, T2> InstantiatePromise<T1, T2>()
        {
            CommandPromise<T1, T2> result;

            if (IsOnceOff)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise<T1, T2>>();
                result = pool.GetInstance();
                result.PoolBinder = _poolBinder;
                result.IsOnceOff = IsOnceOff;
                result.ReleaseHandler = HandleResolve;
            }
            else
            {
                result = new CommandPromise<T1, T2>();
                result.IsOnceOff = IsOnceOff;
            }

            return result;
        }

        private ICommandPromise<PromisedT>[] InstantiateArrayPromise(ICommand<PromisedT>[] commands)
        {
            var promiseArray = new ICommandPromise<PromisedT>[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise<PromisedT>();
                _promiseList.Add(promiseArray[i]);
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
                promiseArray[i] = InstantiatePromise<PromisedT>();
                _promiseList.Add(promiseArray[i]);
                _promiseList.Add(promiseArray[i]);
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
                promiseArray[i] = InstantiatePromise<PromisedT>();
                _promiseList.Add(promiseArray[i]);
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
        }

        private void ResolveRelease()
        {
            if (IsOnceOff)
            {
                ReleasePromise();
                Dispose();
                BindingStatus = CommandBindingStatus.Released;
            }
        }

        private void ReleasePromise()
        {
            var values = Value as object[];
            foreach (var item in values)
            {
                var poolable = item as IPoolable;
                poolable.Release();
            }

            foreach (var item in _promiseList)
            {
                item.Release();
            }
        }
        #endregion
    }
}
