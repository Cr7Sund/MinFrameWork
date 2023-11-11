using Cr7Sund.Framework.Api;
using UnityEngine;

namespace Cr7Sund.Framework.Impl
{
    public class ContextView : MonoBehaviour, IContextView
    {
        public IContext Context { get; set; }

        /// <summary>
        /// When a ContextView is Destroyed, automatically removes the associated Context.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Context != null && Impl.Context.FirstContext != null)
            {
                Impl.Context.FirstContext.RemoveContext(Context);
            }
        }
        #region IView implementation

        public bool RequiresContext { get; set; }

        public bool RegisteredWithContext { get; set; }

        public bool AutoRegisterWithContext { get; set; }

        #endregion  
    }
}