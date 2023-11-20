using System;
using System.Collections.Generic;
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

            var values = binding.Value as object[];

            AssertUtil.Greater(values.Length, 0);

            float sliceLength = 1 / values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                var command = values[i] as ISequence;

                command.SliceLength = sliceLength;
                command.SequenceID = i;
            }

            var firstCommand = values[0] as ICommandPromise;
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
            return this.GetBinding(key, null) as ICommandPromiseBinding;
        }
        #endregion
    }

}