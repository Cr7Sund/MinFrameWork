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

            AssertUtil.AreNotEqual(CommandBindingStatus.Running, binding.BindingStatus, new PromiseException(
                "can not react again when running", PromiseExceptionType.CAN_NOT_REACT_RUNNING));
            AssertUtil.AreNotEqual(CommandBindingStatus.Released, binding.BindingStatus, new PromiseException(
                "can not react again since using at once", PromiseExceptionType.CAN_NOT_REACT_RELEASED));


            object[] values = binding.Value as object[];
            AssertUtil.Greater(values.Length, 0, new PromiseException(
                "can not react a empty promise command", PromiseExceptionType.EMPTY_PROMISE_TOREACT));

            binding.RestartPromise();
            binding.RunPromise();

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            // var lastCommand = values[^1] as ICommandPromise<PromisedT>;
            // lastCommand.Catch((ex) => UnityEngine.Debug.Log(ex));

            ICommandPromise<PromisedT> firstCommand = binding.FirstPromise;
            firstCommand.Resolve(data);
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
