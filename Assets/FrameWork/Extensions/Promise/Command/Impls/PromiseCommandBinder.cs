using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using NUnit.Framework;

namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinder : Binder, IPromiseCommandBinder
    {
        [Inject]
        private IInjectionBinder _injectionBinder;


        public void ReactTo(object trigger)
        {
            var binding = GetBinding(trigger);

            var values = binding.Value as object[];

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;
                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            var firstCommand = values[0] as ICommandPromise;
            firstCommand.Resolve();
        }

        public void ReactTo<PromisedT>(object trigger, PromisedT data)
        {
            var binding = GetBinding(trigger);

            var values = binding.Value as object[];

            Assert.Greater(values.Length, 0);

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            var firstCommand = values[0] as ICommandPromise<PromisedT>;
            firstCommand.Resolve(data);
        }


        #region IBinder implementation

        protected override IBinding GetRawBinding()
        {
            var binding = new PromiseCommandBinding(Resolver);
            _injectionBinder.Injector.Inject(binding);
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