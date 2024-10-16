using System;
using System.Collections.Generic;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.UILifeTime;
namespace Cr7Sund.Navigation
{
    public class NavController
    {
        private Stack<INavGraph> _stack = new();
        private readonly Fragment _parentFragment;
        private readonly Activity _hostActivity;

        public int Count => _stack.Count;

        private FragmentManager FragmentManager
        {
            get
            {
                if (_parentFragment == null)
                {
                    return _hostActivity.FragmentManager;
                }
                return _parentFragment.FragmentManager;
            }
        }

        internal NavController(Activity activity)
        {
            _hostActivity = activity;
        }

        internal NavController(Activity activity, Fragment fragment) : this(activity)
        {
            _parentFragment = fragment;
        }

        public bool IsEmpty() => _stack.Count <= 0;
        public INavGraph GetTopStack()
        {
            return _stack.Pop();
        }

        public async PromiseTask Navigate<T>(object context = null) where T : INavGraph, new()
        {
            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            INavGraph route = TransitionRouteFactory.Create<T>();
            var relativeGUID = string.Empty;
            if (!IsEmpty())
            {
                INavGraph topRoute = _stack.Peek();
                relativeGUID = topRoute.Name;
            }

            transaction.StartTransition(relativeGUID);

            if (!IsEmpty())
            {
                var topRoute = _stack.Peek();
                // don't need to consider grandchilds recursively
                // let child lifecycle handle them themselves
                foreach (var fragment in topRoute.RouteFragments)
                {
                    transaction.RemoveRoute(fragment);
                    FragmentManager.Remove(fragment);
                }
            }

            foreach (var navGraph in route.FragmentTypes)
            {
                var fragment = FragmentManager.Create(navGraph, _hostActivity, _parentFragment);
                transaction.AddRoute(fragment, context);
                FragmentManager.Add(fragment);
                route.RouteFragments.Add(fragment);
            }

            try
            {
                await transaction.Commit();
            }
            finally
            {
                TransactionFactory.Return(transaction);
            }

            _stack.Push(route);
        }

        //if not item left in stack, please use parentFragmentManager.FindNav();
        public async PromiseTask<bool> PopBack()
        {
            if (IsEmpty())
            {
                return false;
            }
            var topRoute = _stack.Pop();

            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            transaction.StartTransition(topRoute.Name);

            foreach (var fragment in topRoute.RouteFragments)
            {
                transaction.RemoveRoute(fragment);
                FragmentManager.Remove(fragment);
            }
            topRoute = _stack.Peek();
            foreach (var fragment in topRoute.RouteFragments)
            {
                transaction.AddRoute(fragment);
                FragmentManager.Add(fragment);
            }

            try
            {
                await transaction.PopBack();
            }
            finally
            {
                TransactionFactory.Return(transaction);
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

        public async PromiseTask SwitchNavCtrl(NavController relativeNavController, bool push)
        {
            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            var relativeGUID = string.Empty;
            if (_stack.Count > 0)
            {
                INavGraph topRoute = _stack.Peek();
                relativeGUID = topRoute.Name;
            }
            transaction.StartTransition(relativeGUID);

            if (!IsEmpty())
            {
                var topRoute = _stack.Peek();
                foreach (var fragment in topRoute.RouteFragments)
                {
                    transaction.RemoveRoute(fragment);
                    // FragmentManager.Remove(fragment);
                }
            }

            if (!relativeNavController.IsEmpty())
            {
                var topRoute = relativeNavController._stack.Peek();
                foreach (var fragment in topRoute.RouteFragments)
                {
                    transaction.AddRoute(fragment);
                    // FragmentManager.Remove(fragment);
                }
            }

            try
            {
                await transaction.Commit(push);
            }
            finally
            {
                TransactionFactory.Return(transaction);
            }
        }

        public async PromiseTask Remove(IFragmentKey fragmentKey)
        {
            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            transaction.StartTransition(string.Empty);

            var fragment = FragmentManager.FindFragment(fragmentKey);
            AssertUtil.NotNull(fragment);
            transaction.RemoveRoute(fragment);
            FragmentManager.Remove(fragment);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                TransactionFactory.Return(transaction);
            }
        }

        public async PromiseTask Replace(IFragmentKey newFragmentKey, IFragmentKey replaceFragmentKey, object intent = null)
        {
            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            transaction.StartTransition(replaceFragmentKey.ViewType.Name);

            var fragment = FragmentManager.FindFragment(replaceFragmentKey);
            transaction.RemoveRoute(fragment);
            FragmentManager.Remove(fragment);

            var newFragment = FragmentManager.Create(
                newFragmentKey,
                _hostActivity, _parentFragment);
            transaction.AddRoute(newFragment, intent);
            FragmentManager.Add(newFragment);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                TransactionFactory.Return(transaction);
            }
        }

        public async PromiseTask Add(IFragmentKey fragmentKey, object intent = null)
        {
            ITransaction transaction = TransactionFactory.Create<UITransaction>();
            transaction.StartTransition(string.Empty);

            var fragment = FragmentManager.Create(
                fragmentKey,
                _hostActivity, _parentFragment);
            transaction.AddRoute(fragment, intent);
            FragmentManager.Add(fragment);
            try
            {
                await transaction.Commit(true);
            }
            finally
            {
                TransactionFactory.Return(transaction);
            }
        }
    }

}
