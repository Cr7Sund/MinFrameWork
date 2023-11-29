using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinder : Binder, ICommandPromiseBinder
    {
        [Inject]
        private IInjectionBinder _injectionBinder;


        public void ReactTo(object trigger)
        {
            ICommandPromiseBinding binding = GetBinding(trigger);

            // AssertUtil.AreNotEqual(CommandBindingStatus.Running, binding.BindingStatus, new PromiseException(
            //     "can not react again when running", PromiseExceptionType.CAN_NOT_REACT_RUNNING));
            // AssertUtil.AreNotEqual(CommandBindingStatus.Released, binding.BindingStatus, new PromiseException(
            //     "can not react again since using at once", PromiseExceptionType.CAN_NOT_REACT_RELEASED));


            binding.RestartPromise();
            binding.RunPromise();
        }


        #region IBinder implementation
        protected override IBinding GetRawBinding()
        {
            var binding = new CommandPromiseBinding(Resolver);
            _injectionBinder.Injector.Inject(binding);
            return binding;
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
