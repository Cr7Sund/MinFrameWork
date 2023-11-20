using System;
using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinding<PromisedT> : Binding, ICommandPromiseBinding<PromisedT>
    {
        [Inject] private IPoolBinder poolBinder;
        [Inject] private IInjectionBinder injectionBinder;
        [Inject] private ICommandBinder _commandBinder;

        public bool UsePooling { get; private set; }


        public CommandPromiseBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            this.ValueConstraint = BindingConstraintType.MANY;
        }

        #region IPromiseCommandBinding<PromisedT> Implementation

        public new ICommandPromiseBinding<PromisedT> To(object value)
        {
            return base.To(value) as ICommandPromiseBinding<PromisedT>;
        }

        public ICommandPromiseBinding<PromisedT> AsPool()
        {
            UsePooling = true;
            return this;
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
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenAny<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenAny<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace(
            params ICommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenRace<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst(
            params ICommand<PromisedT>[] commands)
        {
            var promiseArray = InstantiateValuePromise(commands);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst<T1, T2>()
        {
            InstantiateValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding<PromisedT> ICommandPromiseBinding<PromisedT>.ThenFirst<T1, T2, T3>()
        {
            InstantiateValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenFirst(promiseArray, commands);
            return To(nextPromise);
        }

        #endregion

        #region private methods

        private ICommandPromise<PromisedT> ThenRace(CommandPromise<PromisedT>[] promiseArray,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            return prevValue.ThenRace(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenFirst(CommandPromise<PromisedT>[] promiseArray,
            IEnumerable<ICommand<PromisedT>> commands)
        {
            var prevValue = FindPrevChainPromise<PromisedT>();
            return prevValue.ThenFirst(promiseArray, commands);
        }

        private ICommandPromise<PromisedT> ThenAny(CommandPromise<PromisedT>[] promiseArray,
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

        private CommandPromise<T> FindPrevChainPromise<T>()
        {
            var values = Value as object[];
            if (Value != null)
            {
                AssertUtil.IsInstanceOf<CommandPromise<T>>(values[values.Length - 1]);
                var prevValue = (CommandPromise<T>)values[values.Length - 1];
                return prevValue;
            }
            else
            {
                var firstValue = InstantiatePromise<T>();
                To(firstValue);
                return firstValue;
            }
        }

        private CommandPromise<T> InstantiatePromise<T>()
        {
            CommandPromise<T> result = null;

            if (UsePooling)
            {
                var pool = poolBinder.GetOrCreate<CommandPromise<T>>();
                result = pool.GetInstance();
                if (result.IsRetain)
                {
                    injectionBinder.Injector.Inject(result);
                }
            }
            else
            {
                result = new CommandPromise<T>();
            }

            return result;
        }

        private CommandPromise<T1, T2> InstantiatePromise<T1, T2>()
        {
            CommandPromise<T1, T2> result = null;

            if (UsePooling)
            {
                var pool = poolBinder.GetOrCreate<CommandPromise<T1, T2>>();
                result = pool.GetInstance();
                if (result.IsRetain)
                {
                    injectionBinder.Injector.Inject(result);
                }
            }
            else
            {
                result = new CommandPromise<T1, T2>();
            }

            return result;
        }

        private T InstantiateCommand<T>() where T : class, IBaseCommand
        {
            T result = null;

            result = _commandBinder.Get<T>();
            if (!UsePooling || result is IPoolable { IsRetain: false })
            {
                injectionBinder.Injector.Inject(result);
            }

            return result;
        }

        private CommandPromise<PromisedT>[] InstantiateValuePromise(ICommand<PromisedT>[] commands)
        {
            var promiseArray = new CommandPromise<PromisedT>[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise<PromisedT>();
            }

            return promiseArray;
        }

        private void InstantiateValuePromise<T1, T2>(out List<ICommand<PromisedT>> commands,
            out CommandPromise<PromisedT>[] promiseArray)
            where T1 : class, ICommand<PromisedT>, new()
            where T2 : class, ICommand<PromisedT>, new()
        {
            commands = new List<ICommand<PromisedT>>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>(),
            };
            promiseArray = new CommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise<PromisedT>();
            }
        }

        private void InstantiateValuePromise<T1, T2, T3>(out List<ICommand<PromisedT>> commands,
            out CommandPromise<PromisedT>[] promiseArray)
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
            promiseArray = new CommandPromise<PromisedT>[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise<PromisedT>();
            }
        }

        #endregion
    }
}