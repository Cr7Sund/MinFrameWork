using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
     public abstract class BaseUIController : BaseController, IUIController
     {
          public virtual IPromise Prepare(object intent = null)
          {
               try
               {
                    return OnPrepare(intent);
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise WillPushEnter()
          {
               try
               {
                    return OnWillPushEnter();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise DidPushEnter()
          {
               try
               {
                    return OnDidPushEnter();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise WillPushExit()
          {
               try
               {
                    return OnWillPushExit();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise DidPushExit()
          {
               try
               {
                    return OnDidPushExit();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise WillPopEnter()
          {
               try
               {
                    return OnWillPopEnter();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise DidPopEnter()
          {
               try
               {
                    return OnDidPopEnter();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise WillPopExit()
          {
               try
               {
                    return OnWillPopExit();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          public virtual IPromise DidPopExit()
          {
               try
               {
                    return OnDidPopExit();
               }
               catch (System.Exception e)
               {
                    return Promise.Rejected(e);
               }
          }

          #region overload methods

          protected virtual IPromise OnPrepare(object intent)
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnWillPushEnter()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnDidPushEnter()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnWillPushExit()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnDidPushExit()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnWillPopEnter()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnDidPopEnter()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnWillPopExit()
          {
               return Promise.Resolved();
          }

          protected virtual IPromise OnDidPopExit()
          {
               return Promise.Resolved();
          }

          #endregion
     }
}
