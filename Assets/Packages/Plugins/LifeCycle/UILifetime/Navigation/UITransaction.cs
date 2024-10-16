using System;
using System.Collections.Generic;
using Cr7Sund.Navigation;
using UnityEditor;
namespace Cr7Sund.UILifeTime
{

    public class UITransaction : ITransaction
    {
        private List<Fragment> _showFragments = new();
        private List<Fragment> _hideFragments = new();
        private List<object> _contexts = new();
        private string _relativeUI;

        public void StartTransition(string relativeUI)
        {
            _relativeUI = relativeUI;

            _contexts.Clear();
            _showFragments.Clear();
            _hideFragments.Clear();
        }

        public void AddRoute(Fragment fragment, object intent)
        {
            _contexts.Add(intent);
            AddRoute(fragment);
        }

        public void AddRoute(Fragment fragment)
        {
            // dont detach, so skip context
            _showFragments.Add(fragment);
        }

        public void RemoveRoute(Fragment fragment)
        {
            _hideFragments.Add(fragment);
        }


        public PromiseTask Commit(bool parallelAnimation = true)
        {
            return CommitInternal(true, parallelAnimation);
        }

        public PromiseTask PopBack(bool parallelAnimation = true)
        {
            return CommitInternal(false, parallelAnimation);
        }

        private async PromiseTask CommitInternal(bool push, bool parallelAnimation = true)
        {
            var promiseTasks = new List<PromiseTask>();

            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                fragment.StartAdd();
            }

            var attachTask = AttachTask(promiseTasks);
            var loadTask = LoadTask(promiseTasks);

            try
            {
                await attachTask;
            }
            catch
            {
                // ignored  
            }
            try
            {
                await loadTask;
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Console.Error(ex);
                }
                // ignored
            }

            await VisibleTask();
            await InvisibleTask();

            if (parallelAnimation)
            {
                var hideTask = HideTask(push);
                var showTask = ShowTask(push, promiseTasks);
                await hideTask;
                await showTask;
            }
            else
            {
                await HideTask(push);
                await ShowTask(push, promiseTasks);
            }

            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                fragment.EndAdd();
            }
        }

        private PromiseTask ShowTask(bool push, List<PromiseTask> promiseTasks)
        {
            promiseTasks.Clear();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                promiseTasks.Add(fragment.Show(_relativeUI, push));
                // the view should not depend on other view
            }
            var showTask = PromiseTask.WhenAll(promiseTasks);
            return showTask;
        }

        private async PromiseTask VisibleTask()
        {
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                try
                {
                    await fragment.Visible();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private PromiseTask LoadTask(List<PromiseTask> promiseTasks)
        {
            promiseTasks.Clear();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                promiseTasks.Add(fragment.Load());
                // the view should not depend on other view
            }
            var loadTask = PromiseTask.WhenAll(promiseTasks);
            return loadTask;
        }

        private PromiseTask AttachTask(List<PromiseTask> promiseTasks)
        {
            promiseTasks.Clear();
            for (int index = 0; index < _showFragments.Count; index++)
            {
                var fragment = _showFragments[index];
                var fragmentContext = _contexts[index];
                promiseTasks.Add(fragment.Attach(fragmentContext));
            }
            _contexts.Clear();
            var attachTask = PromiseTask.WhenAll(promiseTasks);
            return attachTask;
        }

        private async PromiseTask InvisibleTask()
        {
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                fragment.StartRemove();
            }

            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                try
                {
                    await fragment.Invisible();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private async PromiseTask HideTask(bool push)
        {
            var promiseTasks = new List<PromiseTask>();
            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                promiseTasks.Add(fragment.Hide(_relativeUI, push, false));
            }
            await PromiseTask.WhenAll(promiseTasks);

            for (int index = 0; index < _hideFragments.Count; index++)
            {
                var fragment = _hideFragments[index];
                fragment.EndRemove();
            }
        }

    }

}
