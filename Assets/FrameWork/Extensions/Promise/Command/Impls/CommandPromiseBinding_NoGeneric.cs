using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using System;
using Cr7Sund.Framework.Util;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinding : Binding, ICommandPromiseBinding
    {
        [Inject] private ICommandBinder _commandBinder;

        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;
        private List<ICommandPromise> _promiseList = new List<ICommandPromise>();
        private ICommandPromise _firstPromise;
        private IDisposable _lastPromise;

        public bool UsePooling { get; private set; }
        public bool IsOnceeOff { get; private set; }
        public CommandBindingStatus BindingStatus { get; private set; }



        public CommandPromiseBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            ValueConstraint = BindingConstraintType.MANY;
            KeyConstraint = BindingConstraintType.ONE;
        }


        #region IPromiseCommandBinding Implementation
        public ICommandPromiseBinding AsPool()
        {
            UsePooling = true;
            return this;
        }

        public ICommandPromiseBinding AsOnce()
        {
            IsOnceeOff = true;
            return this;
        }

        public void RestartPromise()
        {
            if (IsOnceeOff) return;
            
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

        public void RunPromise()
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

            _lastPromise?.Dispose();

            var lastpromise = values[^1] as IBasePromise;
            _lastPromise = lastpromise.Done();

            _firstPromise.Resolve();
        }

        public override void Dispose()
        {
            base.Dispose();
            _promiseList.Clear();
            _firstPromise = null;
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
            if (Value is object[] values)
            {
                var prevValue = (ICommandPromise)values[^1];
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

            if (UsePooling)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise>();
                result = pool.GetInstance();
                result.PoolBinder = _poolBinder;
                result.IsOnceOff = IsOnceeOff;
                result.ReleaseHandler = HandleResolve;
                result.ErrorHandler = HandleRejected;
            }
            else
            {
                result = new CommandPromise();
                result.IsOnceOff = IsOnceeOff;
                result.ReleaseHandler = HandleResolve;
                result.ErrorHandler = HandleRejected;
            }

            return result;
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
            }
        }

        private ICommandPromise[] InstantiateArrayPromise(ICommand[] commands)
        {
            var promiseArray = new ICommandPromise[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
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
        }

        private void ResolveRelease()
        {
            if (UsePooling && IsOnceeOff)
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
