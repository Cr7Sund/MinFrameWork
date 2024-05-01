using Cr7Sund.NodeTree.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.Impl;
using System;

namespace Cr7Sund.Server.UI.Impl
{
     public abstract class BaseUIController : BaseController, IUIController
     {
          [Inject(ServerBindDefine.UILogger)] protected IInternalLog _log;
          protected override IInternalLog Debug => _log;

          public virtual PromiseTask Prepare(object intent = null)
          {
               try
               {
                    return OnPrepare(intent);
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask WillPushEnter()
          {
               try
               {
                    return OnWillPushEnter();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask DidPushEnter()
          {
               try
               {
                    return OnDidPushEnter();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask WillPushExit()
          {
               try
               {
                    return OnWillPushExit();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask DidPushExit()
          {

               try
               {
                    return OnDidPushExit();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask WillPopEnter()
          {

               try
               {
                    return OnWillPopEnter();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask DidPopEnter()
          {

               try
               {
                    return OnDidPopEnter();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask WillPopExit()
          {

               try
               {
                    return OnWillPopExit();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
                    }
               }
          }

          public PromiseTask DidPopExit()
          {
               try
               {
                    return OnDidPopExit();
               }
               catch (Exception e)
               {
                    if (e is OperationCanceledException)
                    {
                         throw;
                    }
                    else
                    {
                         Debug.Error(e);
                         return PromiseTask.CompletedTask;
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
