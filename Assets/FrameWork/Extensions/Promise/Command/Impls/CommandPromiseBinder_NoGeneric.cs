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
            var binding = GetBinding(trigger);

            object[] values = binding.Value as object[];

            AssertUtil.Greater(values.Length, 0, new PromiseException(
                "can not react a empty promise command", PromiseExceptionType.EMPTY_PROMISE_TOREACT));

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            var firstCommand = binding.FirstPromise;
            firstCommand.Resolve();
            firstCommand.Release();
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
