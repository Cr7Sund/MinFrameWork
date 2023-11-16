using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using NUnit.Framework;


namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinding : Binding, IPromiseCommandBinding
    {
        [Inject] private IPoolBinder _poolBinder;
        [Inject] private IInjectionBinder _injectionBinder;

        public bool UsePooling { get; set; }

        public PromiseCommandBinding(Binder.BindingResolver resolver)
        {
            this.resolver = resolver;
            this.ValueConstraint = BindingConstraintType.MANY;
        }

        #region IPromiseCommandBinding Implementation

        public new IPromiseCommandBinding To(object value)
        {
            return base.To(value) as IPromiseCommandBinding;
        }

        IPromiseCommandBinding IPromiseCommandBinding.Then<T>()
        {
            var nextPromise = Instantiate<CommandPromise>();
            var nextCommand = Instantiate<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenAny(params IPromiseCommand[] commands)
        {
            var promiseArray = InstantiateNoValuePromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenAny<T1, T2>()
        {
            InstantiateNoValuePromise<T1, T2>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenAny<T1, T2, T3>()
        {
            InstantiateNoValuePromise<T1, T2, T3>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenRace(params IPromiseCommand[] commands)
        {
            var promiseArray = InstantiateNoValuePromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenRace<T1, T2>()
        {
            InstantiateNoValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding IPromiseCommandBinding.ThenRace<T1, T2, T3>()
        {
            InstantiateNoValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }


        #endregion

        #region private methods


        private ICommandPromise ThenRace(ICommandPromise[] promiseArray, IEnumerable<IPromiseCommand> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();
            return prevValue.ThenRace(promiseArray, commands);
        }


        private ICommandPromise ThenAny(ICommandPromise[] promiseArray, IEnumerable<IPromiseCommand> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();

            return prevValue.ThenAny(promiseArray, commands);
        }

        private void Then(ICommandPromise nextPromise, IPromiseCommand nextCommand)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();

            prevValue.Then(nextPromise, nextCommand);
        }


        private T FindPrevChainPromise<T>() where T : class, new()
        {
            var values = Value as object[];
            if (Value != null)
            {
                Assert.IsInstanceOf<T>(values[values.Length - 1]);
                var prevValue = (T)values[values.Length - 1];
                return prevValue;
            }
            else
            {
                var firstValue = new T();
                To(firstValue);
                return firstValue;
            }
        }

        private T Instantiate<T>() where T : class, new()
        {
            T result = null;

            if (UsePooling)
            {
                var pool = _poolBinder.Get<T>();
                result = pool.GetInstance() as T;
            }
            else
            {
                result = new T();
            }

            if (!UsePooling || (result is IPoolable poolable && poolable.IsRetain))
            {
                _injectionBinder.Injector.Inject(result);
            }

            return result;
        }


        private void InstantiateNoValuePromise<T1, T2>(out List<IPromiseCommand> commands, out CommandPromise[] promiseArray)
            where T1 : class, IPromiseCommand, new()
            where T2 : class, IPromiseCommand, new()
        {
            commands = new List<IPromiseCommand>
            {
                Instantiate<T1>(),
                Instantiate<T2>(),
            };
            promiseArray = new CommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise>();
            }
        }

        private void InstantiateNoValuePromise<T1, T2, T3>(out List<IPromiseCommand> commands, out CommandPromise[] promiseArray)
            where T1 : class, IPromiseCommand, new()
            where T2 : class, IPromiseCommand, new()
            where T3 : class, IPromiseCommand, new()
        {
            commands = new List<IPromiseCommand>
            {
                Instantiate<T1>(),
                Instantiate<T2>(),
                Instantiate<T3>(),
            };
            promiseArray = new CommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise>();
            }
        }

        private CommandPromise[] InstantiateNoValuePromise(IPromiseCommand[] commands)
        {
            var promiseArray = new CommandPromise[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise>();
            }

            return promiseArray;
        }

        #endregion
    }

}