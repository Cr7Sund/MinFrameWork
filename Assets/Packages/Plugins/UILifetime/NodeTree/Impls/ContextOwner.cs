using Cr7Sund.IocContainer;
namespace Cr7Sund.LifeTime
{
    public abstract class ContextOwner
    {
        protected IContext _context;
        public bool IsInjected
        {
            get;
            private set;
        }

         #region Inject Config
        public void Inject()
        {
            if (IsInjected)
                return;

            IsInjected = true;

            _context.Inject(this);
        }

        public void Deject()
        {
            if (!IsInjected)
                return;

            IsInjected = false;
        }

        public void AssignContext(IContext context)
        {
            _context = context;
        }
        #endregion
    }
}
