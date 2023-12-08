using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinder : Binder, ICommandPromiseBinder
    {
        [Inject]
        private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;

        public bool UsePooling;

        public void ReactTo(object trigger)
        {
            ICommandPromiseBinding binding = GetBinding(trigger);

            if (binding.BindingStatus == CommandBindingStatus.Running)
            {
                throw new MyException(PromiseExceptionType.CAN_NOT_REACT_RUNNING);
            }
            if (binding.BindingStatus == CommandBindingStatus.Released)
            {
                throw new MyException(PromiseExceptionType.CAN_NOT_REACT_RELEASED);
            }

            binding.RestartPromise();
            binding.RunPromise();
        }


        #region IBinder implementation
        protected override IBinding GetRawBinding()
        {
            CommandPromiseBinding binding;
            if (UsePooling)
            {
                binding = _poolBinder.GetOrCreate<CommandPromiseBinding>().GetInstance();
                binding.resolver = _bindingResolverHandler;
            }
            else
            {
                binding = new CommandPromiseBinding(_bindingResolverHandler);
            }
            _injectionBinder.Injector.Inject(binding);
            return binding;
        }

        protected override void OnUnbind(IBinding binding)
        {
            binding.Dispose();
            if (UsePooling)
            {
                _poolBinder.GetOrCreate<CommandPromiseBinding>().ReturnInstance((CommandPromiseBinding)binding);
            }
        }

        public new virtual ICommandPromiseBinding Bind(object trigger)
        {
            return base.Bind(trigger) as ICommandPromiseBinding;
        }

        public new ICommandPromiseBinding GetBinding(object key)
        {
            return base.GetBinding(key) as ICommandPromiseBinding;
        }

        public new ICommandPromiseBinding GetBinding(object key, object name)
        {
            return base.GetBinding(key, null) as ICommandPromiseBinding;
        }
        #endregion
    }

}
