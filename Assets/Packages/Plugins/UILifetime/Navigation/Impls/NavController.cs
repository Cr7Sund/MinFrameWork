using System.Collections.Generic;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public class NavController : ContextLifeCycle
    {
        // private Stack<IFragmentKey> _stack = new();
        private Stack<IRouteKey> _stack = new();
        [Inject]
        private TransactionFactory _transactionFactory;

        public int Count => _stack.Count;


        private NavController()
        {

        }

        public static NavController CreateNavController() { return new NavController(); }


        public bool IsEmpty() => _stack.Count <= 0;


        #region Navigation
        public async PromiseTask Navigate(IRouteKey targetRoute, IRouteArgs intent = null)
        {
            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);

            var topRoute = GetTopNavGraph();

            transaction.StartTransition(topRoute?.Key, intent);
            transaction.RemoveRoute(topRoute);
            transaction.AddRoute(targetRoute);

            if (topRoute is { IsInStack: true })
            {
                _stack.Pop();
            }

            try
            {
                await transaction.Commit(targetRoute.ParallelTransition);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }

            _stack.Push(targetRoute);
        }

        //if not item left in stack, please use parentFragmentManager.FindNav();
        public async PromiseTask<bool> PopBack()
        {
            if (IsEmpty())
            {
                return false;
            }
            var topRoute = GetTopNavGraph();

            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);
            transaction.StartTransition(topRoute.Key, null);

            transaction.RemoveRoute(topRoute);
            topRoute = GetTopNavGraph();
            transaction.AddRoute(topRoute);

            try
            {
                await transaction.PopBack(topRoute.ParallelTransition);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }
            return true;
        }

        // public async PromiseTask<bool> NavigateUpTo<T>() where T : INavGraph
        // {
        //     while (_stack.Count > 0 &&
        //            !_stack.Peek().GetType().IsInstanceOfType(typeof(T)))
        //     {
        //         var route = _stack.Pop();
        //         await FragmentManager.Push_Invisible();
        //     }
        //     return _stack.Count > 0;
        // }

        public async PromiseTask SwitchNavCtrl(NavController relativeNavController, IRouteArgs intent)
        {
            if (relativeNavController.IsEmpty())
            {
                return;
            }

            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);

            var topRoute = relativeNavController.GetTopNavGraph();
            transaction.StartTransition(topRoute.Key, intent);

            transaction.AddRoute(topRoute);
            topRoute = GetTopNavGraph();
            transaction.RemoveRoute(topRoute);

            try
            {
                await transaction.Commit(topRoute.ParallelTransition);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }
        }

        public async PromiseTask Remove(IRouteKey routeKey)
        {
            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);
            transaction.StartTransition(string.Empty, null);

            transaction.RemoveRoute(routeKey);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }
        }

        public async PromiseTask Replace(IRouteKey newRouteKey, IRouteKey replaceRouteKey, IRouteArgs intent = null)
        {
            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);
            transaction.StartTransition(replaceRouteKey.Key, intent);

            transaction.RemoveRoute(replaceRouteKey);
            transaction.AddRoute(newRouteKey);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }
        }

        //PLAN :Test Open child Panel during open panel,
        //expect append parent status
        // 1.  await load , when parent already load
        // --- Though we iterate  all children,  but we determine the status before invoke action,
        // so it will not affect the parent (previous)transition.
        // 2. don't invoke twice 
        public async PromiseTask Add(IRouteKey routeKey, IRouteArgs intent = null)
        {
            //PLAN how to delete new add task
            ITransaction transaction = _transactionFactory.Create<Transaction>(_context);
            transaction.StartTransition(string.Empty, intent);

            transaction.AddRoute(routeKey);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                _transactionFactory.Return(transaction, _context);
            }
        }

        private IRouteKey GetTopNavGraph()
        {
            if (!IsEmpty())
            {
                IRouteKey topRoute = _stack.Peek();
                return topRoute;
            }

            return null;
        }
#endregion

    }
}
