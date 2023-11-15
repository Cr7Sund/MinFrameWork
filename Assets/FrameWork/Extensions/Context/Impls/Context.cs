using Cr7Sund.Framework.Api;

namespace Cr7Sund.Framework.Impl
{
    public class Context : Binder, IContext
    {
        /// In a multi-Context app, this represents the first Context to instantiate.
        public static IContext FirstContext;

        /// If false, the `Launch()` method won't fire.
        public bool autoStartup;

        /// The top of the View hierarchy.
        /// In MVCSContext, this is your top-level GameObject
        public object contextView;

        public Context(object view, ContextStartupFlags flags)
        {
            //If firstContext was unloaded, the contextView will be null. Assign the new context as firstContext.
            if (FirstContext == null || FirstContext.GetContextView() == null)
            {
                FirstContext = this;
            }
            else
            {
                FirstContext.AddContext(this);
            }
            SetContextView(view);
            AddCoreComponents();
            this.autoStartup = (flags & ContextStartupFlags.MANUAL_LAUNCH) != ContextStartupFlags.MANUAL_LAUNCH;
            if ((flags & ContextStartupFlags.MANUAL_MAPPING) != ContextStartupFlags.MANUAL_MAPPING)
            {
                Start();
            }
        }

        public Context(object view) : this(view, ContextStartupFlags.AUTOMATIC)
        { }
        public Context(object view, bool autoMapping) : this(view, (autoMapping) ? ContextStartupFlags.MANUAL_MAPPING : ContextStartupFlags.MANUAL_LAUNCH | ContextStartupFlags.MANUAL_MAPPING)
        { }

        /// Override to add components. Or just extend MVCSContext.
        protected virtual void AddCoreComponents()
        { }

        /// Override to instantiate components. Or just extend MVCSContext
        protected virtual void InstantiateCoreComponents()
        { }

        /// Override to map project-specific bindings
        protected virtual void MapBindings()
        { }

        /// Override to do things after binding but before app launch
        protected virtual void PostBindings()
        { }

        /// Set the object that represents the top of the Context hierarchy.
        /// In MVCSContext, this would be a GameObject.
        protected virtual IContext SetContextView(object view)
        {
            contextView = view;
            return this;
        }


        #region  IContext Implementation

        public virtual IContext AddContext(IContext context)
        {
            return this;
        }

        public virtual void AddView(object view)
        { }

        public object GetContextView()
        {
            return contextView;
        }

        public virtual IContext RemoveContext(IContext context)
        {
            // If we're removing firsContext , set firstContext to null
            if (context == FirstContext)
            {
                FirstContext = null;
            }
            else
            {
                context.OnRemove();
            }

            return this;
        }

        public virtual void RemoveView(object view)
        { }

        public IContext Start()
        {
            InstantiateCoreComponents();
            MapBindings();
            PostBindings();
            if (autoStartup)
            {
                Launch();
            }

            return this;
        }

        public virtual void Launch()
        { }

        #endregion
    }
}