using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using System.Collections.Generic;
using System.Linq;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinding : Binding, ICommandPromiseBinding
    {
        [Inject] private ICommandBinder _commandBinder;
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;

        private ICommandPromise _firstPromise;

        public bool UsePooling { get; private set; }
        public ICommandPromise FirstPromise => _firstPromise;

        public CommandPromiseBinding(Binder.BindingResolver resolver) : base(resolver)
        {
            ValueConstraint = BindingConstraintType.MANY;
        }


        #region IPromiseCommandBinding Implementation
        public ICommandPromiseBinding AsPool()
        {
            UsePooling = true;
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
            ICommandPromise[] promiseArray = InstantiateNoValuePromise(commands);

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

        private CommandPromise InstantiatePromise()
        {
            CommandPromise result;

            if (UsePooling)
            {
                var pool = _poolBinder.GetOrCreate<CommandPromise>();
                result = pool.GetInstance();
                result.PoolBinder = _poolBinder;
            }
            else
            {
                result = new CommandPromise();
            }

            return result;
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


        private void InstantiateNoValuePromise<T1, T2>(out List<ICommand> commands, out ICommandPromise[] promiseArray)
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

        private void InstantiateNoValuePromise<T1, T2, T3>(out List<ICommand> commands, out ICommandPromise[] promiseArray)
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

        private ICommandPromise[] InstantiateNoValuePromise(ICommand[] commands)
        {
            var promiseArray = new ICommandPromise[commands.Count()];
            for (int i = 0; i < promiseArray.Length; i++)
            {
                promiseArray[i] = InstantiatePromise();
            }

            return promiseArray;
        }
        #endregion
    }

}
