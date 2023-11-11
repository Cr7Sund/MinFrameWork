using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Util;
using NUnit.Framework;


namespace Cr7Sund.Framework.Impl
{
    public class PromiseCommandBinding : Binding, IPromiseCommandBinding
    {
        [Inject] private IPoolBinder poolBinder;
        [Inject] private IInjectionBinder injectionBinder;

        public bool UsePooling
        {
            get
            {
                return this.ValueConstraint == BindingConstraintType.MANY;
            }
            set
            {
                this.ValueConstraint = value ? BindingConstraintType.POOL : BindingConstraintType.MANY;
            }
        }

        public PromiseCommandBinding(Binder.BindingResolver resolver)
        {
            this.resolver = resolver;
            this.ValueConstraint = BindingConstraintType.MANY;
        }

        public IPromiseCommandBinding Then<T>() where T : class, IPromiseCommand, new()
        {
            Assert.IsTrue(typeof(PromiseCommand).IsAssignableFrom(typeof(T)));

            var resultPromise = InstantiateCommand<T>();

            return this.To(resultPromise) as IPromiseCommandBinding;
        }

        private IPromiseCommand InstantiateCommand<T>() where T : class, IPromiseCommand, new()
        {
            IPromiseCommand resultCommand = null;

            if (UsePooling)
            {
                var pool = poolBinder.Get<T>();
                resultCommand = pool.GetInstance() as IPromiseCommand;
            }
            else
            {
                resultCommand = new T();
            }

            if (resultCommand.IsRetain)
            {
                injectionBinder.Injector.Inject(resultCommand);
            }

            return resultCommand;
        }

        public new IPromiseCommandBinding ToName(object name)
        {
            base.ToName(name);
            if (this.Value is IPromiseCommand promiseCommand)
            {
                promiseCommand.WithName(name);
            }
            return this;
        }
    }


}