using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinder<PromisedT> : Binder, ICommandPromiseBinder<PromisedT>
    {
        [Inject] private IInjectionBinder _injectionBinder;


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
            var binding = new CommandPromiseBinding<PromisedT>(Resolver);
            _injectionBinder.Injector.Inject(binding);
            return binding;
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
