using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinding<PromisedT> : Binding, IPromiseCommandBinding<PromisedT>
    {
        [Inject] private IPoolBinder _poolBinder;
        [Inject] private IInjectionBinder _injectionBinder;

        public bool UsePooling { get; set; }

        public PromiseCommandBinding(Binder.BindingResolver resolver)
        {
            this.resolver = resolver;
            this.ValueConstraint = BindingConstraintType.MANY;
        }

        #region IPromiseCommandBinding<PromisedT> Implementation

        public new IPromiseCommandBinding<PromisedT> To(object value)
        {
            return base.To(value) as IPromiseCommandBinding<PromisedT>;
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.Then<T>()
        {
            var nextPromise = Instantiate<CommandPromise<PromisedT>>();
            var nextCommand = Instantiate<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.Then<T, ConvertedT>()
        {
            var nextPromise = Instantiate<CommandPromise<ConvertedT>>();
            var nextCommand = Instantiate<T>();

            Then(nextPromise, nextCommand);

            return To(nextPromise);
        }


        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenConvert<T, ConvertedT>()
        {
            var nextPromise = Instantiate<CommandPromise<PromisedT, ConvertedT>>();
            var nextCommand = Instantiate<T>();

            Then(nextPromise, nextCommand);
            // Assert.NotNull(prevValue, $"the first command try to Convert {typeof(PromisedT)} into {typeof(ConvertedT)}. Currently dont support that. ");

            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenAny(params IPromiseCommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenAny<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenAny<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenRace(params IPromiseCommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenRace<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenRace<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenFirst(params IPromiseCommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenFirst<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        IPromiseCommandBinding<PromisedT> IPromiseCommandBinding<PromisedT>.ThenFirst<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        #endregion

        #region private methods


        private ICommandPromise<PromisedT> ThenRace(CommandPromise<PromisedT>[] promiseArray, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<PromisedT>>();
            return prevValue.ThenRace(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenFirst(CommandPromise<PromisedT>[] promiseArray, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<PromisedT>>();
            return prevValue.ThenFirst(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenAny(CommandPromise<PromisedT>[] promiseArray, IEnumerable<IPromiseCommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<PromisedT>>();
            return prevValue.ThenAny(promiseArray, commands);
        }

        private void Then(ICommandPromise<PromisedT> nextPromise, IPromiseCommand<PromisedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<PromisedT>>();
            prevValue.Then(nextPromise, nextCommand);
        }

        private void Then<ConvertedT>(ICommandPromise<ConvertedT> nextPromise, IPromiseCommand<ConvertedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<ConvertedT>>();
            prevValue.Then(nextPromise, nextCommand);
        }

        private void Then< ConvertedT>(CommandPromise<PromisedT, ConvertedT> nextPromise, IPromiseCommand<PromisedT, ConvertedT> nextCommand)
        {
            var prevValue = FindPrevChainPromise<CommandPromise<PromisedT>>();

            prevValue.Then(nextPromise, nextCommand);
        }

        private T FindPrevChainPromise<T>() where T : class, new()
        {
            var values = Value as object[];
            if (Value != null)
            {
                AssertUtil.IsInstanceOf<T>(values[values.Length - 1]);
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

        private CommandPromise<PromisedT>[] InstantiateValuePromise(IPromiseCommand<PromisedT>[] commands)
        {
            var promiseArray = new CommandPromise<PromisedT>[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise<PromisedT>>();
            }

            return promiseArray;
        }

        private void InstantiateValuePromise<T1, T2>(out List<IPromiseCommand<PromisedT>> commands, out CommandPromise<PromisedT>[] promiseArray)
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
        {
            commands = new List<IPromiseCommand<PromisedT>>
            {
                Instantiate<T1>(),
                Instantiate<T2>(),
            };
            promiseArray = new CommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise<PromisedT>>();
            }
        }

        private void InstantiateValuePromise<T1, T2, T3>(out List<IPromiseCommand<PromisedT>> commands, out CommandPromise<PromisedT>[] promiseArray)
            where T1 : class, IPromiseCommand<PromisedT>, new()
            where T2 : class, IPromiseCommand<PromisedT>, new()
            where T3 : class, IPromiseCommand<PromisedT>, new()
        {
            commands = new List<IPromiseCommand<PromisedT>>
            {
                Instantiate<T1>(),
                Instantiate<T2>(),
                Instantiate<T3>()
            };
            promiseArray = new CommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = Instantiate<CommandPromise<PromisedT>>();
            }
        }


        #endregion
    }

}