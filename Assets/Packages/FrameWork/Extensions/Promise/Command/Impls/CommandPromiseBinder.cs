using Cr7Sund.Package.Api;
using Cr7Sund.FrameWork.Util;
namespace Cr7Sund.Package.Impl
{
    public class CommandPromiseBinder<PromisedT> : Binder, ICommandPromiseBinder<PromisedT>
    {
        [Inject] private IInjectionBinder _injectionBinder;
        [Inject] private IPoolBinder _poolBinder;

        public bool UsePooling;

        public void ReactTo(object trigger, PromisedT data)
        {
            ICommandPromiseBinding<PromisedT> binding = GetBinding(trigger);

            if (binding.BindingStatus == CommandBindingStatus.Running)
            {
                throw new MyException(PromiseExceptionType.CAN_NOT_REACT_RUNNING);
            }
            if (binding.BindingStatus == CommandBindingStatus.Released)
            {
                throw new MyException(PromiseExceptionType.CAN_NOT_REACT_RELEASED);
            }

            binding.RestartPromise();
            binding.RunPromise(data);
        }


        #region IBinder implementation
        protected override IBinding GetRawBinding()
        {
            CommandPromiseBinding<PromisedT> binding;
            if (UsePooling)
            {
                binding = _poolBinder.GetOrCreate<CommandPromiseBinding<PromisedT>>().GetInstance();
                binding.resolver = _bindingResolverHandler;
            }
            else
            {
                binding = new CommandPromiseBinding<PromisedT>(_bindingResolverHandler);
            }
            _injectionBinder.Injector.Inject(binding);
            return binding;
        }

        protected override void OnUnbind(IBinding binding)
        {
            binding.Dispose();
            if (UsePooling)
            {
                _poolBinder.GetOrCreate<CommandPromiseBinding<PromisedT>>().ReturnInstance((CommandPromiseBinding<PromisedT>)binding);
            }
        }
        public new virtual ICommandPromiseBinding<PromisedT> Bind(object trigger)
        {
            return base.Bind(trigger) as ICommandPromiseBinding<PromisedT>;
        }

        public new ICommandPromiseBinding<PromisedT> GetBinding(object key)
        {
            return base.GetBinding(key) as ICommandPromiseBinding<PromisedT>;
        }

        public new ICommandPromiseBinding<PromisedT> GetBinding(object key, object name)
        {
            return base.GetBinding(key, null) as ICommandPromiseBinding<PromisedT>;
        }
        #endregion

    }
}
