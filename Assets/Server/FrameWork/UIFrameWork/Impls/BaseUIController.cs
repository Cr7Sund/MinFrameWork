using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Impl;


namespace Cr7Sund.Server.UI.Impl
{
     public abstract class BaseUIController : BaseController, IUIController
     {
          [Inject(ServerBindDefine.UILogger)] protected IInternalLog Debug;

          public virtual IPromise Prepare(object intent = null)
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnPrepare(intent);
               }
               else
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
          }

          public IPromise WillPushEnter()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnWillPushEnter();
               }
               else
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
          }

          public IPromise DidPushEnter()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnDidPushEnter();
               }
               else
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
          }

          public IPromise WillPushExit()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnWillPushExit();
               }
               else
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
          }

          public IPromise DidPushExit()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnDidPushExit();
               }
               else
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
          }

          public IPromise WillPopEnter()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnWillPopEnter();
               }
               else
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
          }

          public IPromise DidPopEnter()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnDidPopEnter();
               }
               else
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
          }

          public IPromise WillPopExit()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnWillPopExit();
               }
               else
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
          }

          public IPromise DidPopExit()
          {
               if (MacroDefine.NoCatchMode)
               {
                    return OnDidPopExit();
               }
               else
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
