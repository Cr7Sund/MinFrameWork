using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinder : Binder, IPromiseCommandBinder
    {
        [Inject]
        private IInjectionBinder injectionBinder;

        [Inject]
        public IPoolBinder poolBinder;

        public void ReactTo(object trigger)
        {
            var binding = GetBinding(trigger);

            var values = binding.Value as object[];
            var command = ResolvedPromiseCommand.Resolved();

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                command = command.Then(values[i] as IPromiseCommand);

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }
        }

        public void ReactTo<PromisedT>(object trigger, PromisedT data)
        {
            var binding = GetBinding(trigger);

            var values = binding.Value as object[];
            var command = ResolvedPromiseCommand<PromisedT>.Resolved(data);

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                command = command.Then(values[i] as IPromiseCommand<PromisedT>);

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }
        }

        #region IBinder implementation

        protected override IBinding GetRawBinding()
        {
            var binding = new PromiseCommandBinding(Resolver);
            injectionBinder.Injector.Inject(binding);
            return binding;
        }

        public new virtual IPromiseCommandBinding Bind(object trigger)
        {
            return base.Bind(trigger) as IPromiseCommandBinding;
        }

        public new IPromiseCommandBinding GetBinding(object key)
        {
            return base.GetBinding(key) as IPromiseCommandBinding;
        }

        public new IPromiseCommandBinding GetBinding(object key, object name)
        {
            return this.GetBinding(key, null) as IPromiseCommandBinding;
        }
        #endregion
    }
}