using System.Collections.Generic;
using System.Linq;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;


namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinding : Binding, ICommandPromiseBinding
    {
        [Inject] private IPoolBinder _poolBinder;
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private ICommandBinder _commandBinder;
        
        public bool UsePooling { get; private set; }

        public CommandPromiseBinding(Binder.BindingResolver resolver):base(resolver)
        {
            this.ValueConstraint = BindingConstraintType.MANY;
        }

        #region IPromiseCommandBinding Implementation

        public ICommandPromiseBinding AsPool()
        {
            this.UsePooling = true;
            return this;
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
            var promiseArray = InstantiateNoValuePromise(commands);

            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenAny<T1, T2>()
        {
            InstantiateNoValuePromise<T1, T2>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenAny<T1, T2, T3>()
        {
            InstantiateNoValuePromise<T1, T2, T3>(out var commands, out var promiseArray);
            var nextPromise = ThenAny(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace(params ICommand[] commands)
        {
            var promiseArray = InstantiateNoValuePromise(commands);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace<T1, T2>()
        {
            InstantiateNoValuePromise<T1, T2>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }

        ICommandPromiseBinding ICommandPromiseBinding.ThenRace<T1, T2, T3>()
        {
            InstantiateNoValuePromise<T1, T2, T3>(out var commands, out var promiseArray);

            var nextPromise = ThenRace(promiseArray, commands);
            return To(nextPromise);
        }


        #endregion

        #region private methods


        private ICommandPromise ThenRace(ICommandPromise[] promiseArray, IEnumerable<ICommand> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();
            return prevValue.ThenRace(promiseArray, commands);
        }


        private ICommandPromise ThenAny(ICommandPromise[] promiseArray, IEnumerable<ICommand> commands)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();

            return prevValue.ThenAny(promiseArray, commands);
        }

        private void Then(ICommandPromise nextPromise, ICommand nextCommand)
        {
            var prevValue = FindPrevChainPromise<CommandPromise>();

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

        private CommandPromise InstantiatePromise()
        {
            CommandPromise result = null;

            if (UsePooling)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise>();
                result = pool.GetInstance();
                if (result.IsRetain)
                {
                    _injectionBinder.Injector.Inject(result);
                }
            }
            else
            {
                result = new CommandPromise();
            }

            return result;
        }

        private T InstantiateCommand<T>() where T : class, IBaseCommand
        {
            T result = null;

            result = _commandBinder.Get<T>();
            if (!UsePooling || result is IPoolable { IsRetain: false })
            {
                _injectionBinder.Injector.Inject(result);
            }

            return result;
        }


        private void InstantiateNoValuePromise<T1, T2>(out List<ICommand> commands, out CommandPromise[] promiseArray)
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
        {
            commands = new List<ICommand>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>(),
            };
            promiseArray = new CommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
            }
        }

        private void InstantiateNoValuePromise<T1, T2, T3>(out List<ICommand> commands, out CommandPromise[] promiseArray)
            where T1 : class, ICommand, new()
            where T2 : class, ICommand, new()
            where T3 : class, ICommand, new()
        {
            commands = new List<ICommand>
            {
                InstantiateCommand<T1>(),
                InstantiateCommand<T2>(),
                InstantiateCommand<T3>(),
            };
            promiseArray = new CommandPromise[commands.Count];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
            }
        }

        private CommandPromise[] InstantiateNoValuePromise(ICommand[] commands)
        {
            var promiseArray = new CommandPromise[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
            }

            return promiseArray;
        }

        #endregion
    }

}