using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cr7Sund.FrameWork.Util;
using UnityEngine;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public class Transaction : ITransaction
    {
        private IRouteKey _parentKey;
        // route context
        // ---- ----
        private string _relativeUI;
        private IRouteArgs _fragmentContext;

        private readonly List<IRouteKey> _showFragments = new();
        private readonly List<IRouteKey> _hideFragments = new();

        [Inject] private ITransitionContainer _transitionContainer;
        [Inject] private ILoadModule _loadModule;
        [Inject] private IPoolBinder _poolBinder;

        #region Exposer Methods
        public void StartTransition(string relativeUI, IRouteArgs intent)
        {
            _relativeUI = relativeUI;

            _fragmentContext = intent;
            _showFragments.Clear();
            _hideFragments.Clear();
        }

        public void AddRoute(IRouteKey route)
        {
            // dont detach, so skip context
            if (!_showFragments.Contains(route))
            {
                _showFragments.Add(route);
                if (route is INavGraph navGraph)
                {
                    foreach (var childFragment in navGraph.RouteKeys)
                    {
                        AddRoute(childFragment);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"try to add same route {route} twice");
            }
        }

        public void RemoveRoute(IRouteKey route)
        {
            _hideFragments.Add(route);
        }

        public PromiseTask Commit(bool parallelAnimation)
        {
            return CommitInternal(true, parallelAnimation);
        }

        public PromiseTask PopBack(bool parallelAnimation)
        {
            return CommitInternal(false, parallelAnimation);
        }
        #endregion

        #region Internal Methods
        private static IRouteKey FindTopMostLayoutRoot(IRouteKey rect)
        {
            var current = rect;
            var topMost = rect;

            while (current.ParentKey != null)
            {
                var parent = current.ParentKey;
                if (parent == null) break;

                topMost = parent;
                current = parent;
            }
            return topMost;
        }

        private async PromiseTask CommitInternal(bool push, bool parallelAnimation)
        {
            // remove intersect
            for (int index = _showFragments.Count - 1; index >= 0; index--)
            {
                var fragment = _showFragments[index];
                if (_hideFragments.Contains(fragment))
                {
                    _showFragments.RemoveAt(index);
                }
                var topKey = FindTopMostLayoutRoot(fragment);
                if (topKey == _parentKey)
                {
                    _showFragments.RemoveAt(index);
                }
            }

            await StartAdd();
            await StartRemove();

            var preloadTask = PreloadTask();
            var loadTask = LoadTask();
            await preloadTask;
            await loadTask;

            await AttachTask();
            await InvisibleTask();

            await TransitionTask(push, parallelAnimation);

            await VisibleTask();
            await DetachTask();

            EndAdd();
            EndRemove();
        }
        
        private async Task TransitionTask(bool push, bool parallelAnimation)
        {
            if (parallelAnimation)
            {
                var hideTask = HideTask(push);
                var showTask = ShowTask(push);
                await hideTask;
                await showTask;
            }
            else
            {
                await HideTask(push);
                await ShowTask(push);
            }
        }

        private async PromiseTask ShowTask(bool push)
        {
            var promiseTasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                // the view should not depend on other view
                var transition = _transitionContainer.GetTransition(fragment);
                promiseTasks.Add(transition.Show(fragment, _relativeUI, push));
                promiseTasks.Add(_loadModule.AppendTransition(fragment));
            }
            await PromiseTask.WhenAll(promiseTasks);
            _poolBinder.Return(promiseTasks);
        }

        private async PromiseTask VisibleTask()
        {
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                await _loadModule.Visible(fragment);
            }
        }

        private async PromiseTask AttachTask()
        {
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                await _loadModule.Attach(fragment, _fragmentContext);
            }
        }

        private async PromiseTask StartAdd()
        {
            var tasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                tasks.Add(_loadModule.StartAdd(fragment.ParentKey, fragment, _fragmentContext));
            }
            await PromiseTask.WhenAll(tasks);
            _poolBinder.Return(tasks);
        }

        private async PromiseTask StartRemove()
        {
            await Do(async (transaction, index) =>
            {
                var fragment = transaction._hideFragments[index];
                await transaction._loadModule.StartRemove(fragment, fragment.IsInStack);
            });
        }

        private async PromiseTask Do(Func<Transaction, int, PromiseTask> func)
        {
            var tasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                tasks.Add(func.Invoke(this, index));
            }
            await PromiseTask.WhenAll(tasks);
            _poolBinder.Return(tasks);
        }

        private async PromiseTask LoadTask()
        {
            var tasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                tasks.Add(_loadModule.Load(fragment));
            }
            await PromiseTask.WhenAll(tasks);
            _poolBinder.Return(tasks);
        }

        private async PromiseTask PreloadTask()
        {
            var tasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                tasks.Add(_loadModule.Preload(fragment, _fragmentContext));
            }
            await PromiseTask.WhenAll(tasks);
            _poolBinder.Return(tasks);
        }

        private async PromiseTask InvisibleTask()
        {
            var promiseTasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                promiseTasks.Add(_loadModule.Invisible(fragment));
            }
            await PromiseTask.WhenAll(promiseTasks);
            _poolBinder.Return(promiseTasks);
        }

        private async PromiseTask HideTask(bool push)
        {
            var promiseTasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                bool skipAnimation = fragment.SkipHideAnimation;
                if (!skipAnimation)
                {
                    var transition = _transitionContainer.GetTransition(fragment);
                    promiseTasks.Add(transition.Hide(fragment, _relativeUI, push));
                }
            }
            await PromiseTask.WhenAll(promiseTasks);
            _poolBinder.Return(promiseTasks);
        }

        private async PromiseTask DetachTask()
        {
            var promiseTasks = _poolBinder.AutoCreate<List<PromiseTask>>();
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                promiseTasks.Add(_loadModule.DisAttach(fragment, fragment.IsInStack));
            }
            await PromiseTask.WhenAll(promiseTasks);
            _poolBinder.Return(promiseTasks);
        }

        private void EndAdd()
        {
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                _loadModule.EndAdd(fragment);
            }
            _fragmentContext = null;
        }

        private void EndRemove()
        {
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                _loadModule.EndRemove(fragment, fragment.IsInStack);
            }
        }
        #endregion
    }

}
