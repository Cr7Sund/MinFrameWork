using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server.UI.Impl
{
    public abstract class BaseUIController : BaseController, IUIController
     {
          [Inject(ServerBindDefine.UILogger)] protected IInternalLog Debug;

          public virtual PromiseTask Prepare(object intent = null)
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask WillPushEnter()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask DidPushEnter()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask WillPushExit()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask DidPushExit()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask WillPopEnter()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask DidPopEnter()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask WillPopExit()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          public PromiseTask DidPopExit()
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
                         Debug.Error(e);
                         throw;
                    }
               }
          }

          #region overload methods

          protected virtual PromiseTask OnPrepare(object intent)
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnWillPushEnter()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnDidPushEnter()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnWillPushExit()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnDidPushExit()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnWillPopEnter()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnDidPopEnter()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnWillPopExit()
          {
               return PromiseTask.CompletedTask;
          }

          protected virtual PromiseTask OnDidPopExit()
          {
               return PromiseTask.CompletedTask;
          }

          #endregion
     }
}
