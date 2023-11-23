using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
namespace Cr7Sund.Framework.Impl
{
    public class CommandPromiseBinder<PromisedT> : Binder, ICommandPromiseBinder<PromisedT>
    {
        [Inject] private IInjectionBinder _injectionBinder;


        public void ReactTo(object trigger, PromisedT data)
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
            firstCommand.Resolve(data);
            firstCommand.Release();
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
