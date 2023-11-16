using System;
using System.Collections.Generic;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinder<PromisedT> : Binder, IPromiseCommandBinder<PromisedT>
    {
        [Inject]
        private IInjectionBinder _injectionBinder;


        public void ReactTo(object trigger, PromisedT data)
        {
            var binding = GetBinding(trigger);

            var values = binding.Value as object[];

            AssertUtil.Greater(values.Length, 0);

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
            var binding = new PromiseCommandBinding<PromisedT>(Resolver);
            _injectionBinder.Injector.Inject(binding);
            return binding;
        }

        public new virtual IPromiseCommandBinding<PromisedT> Bind(object trigger)
        {
            return base.Bind(trigger) as IPromiseCommandBinding<PromisedT>;
        }

        public new IPromiseCommandBinding<PromisedT> GetBinding(object key)
        {
            return base.GetBinding(key) as IPromiseCommandBinding<PromisedT>;
        }

        public new IPromiseCommandBinding<PromisedT> GetBinding(object key, object name)
        {
            return this.GetBinding(key, null) as IPromiseCommandBinding<PromisedT>;
        }
        #endregion
    }

}