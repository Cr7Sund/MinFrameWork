using System;
using System.Collections.Generic;
namespace Cr7Sund.UILifeTime
{
    public class FragmentManager
    {
        private List<Fragment> _fragments = new();

        public int Count => _fragments.Count;

        #region 管理 Fragment 生命周期
        public void Add(Fragment fragment)
        {
            fragment.CancelRemove();
            _fragments.Add(fragment);
        }

        public void Remove(Fragment fragment)
        {
            fragment.CancelAdd();
            _fragments.Remove(fragment);
        }

        public async void Destroy()
        {
            for (int index = 0; index < _fragments.Count; index++)
            {
                var fragment = _fragments[index];
                fragment.StartRemove();
            }

            var promiseTasks = new List<PromiseTask>();
            for (int index = 0; index < _fragments.Count; index++)
            {
                var fragment = _fragments[index];
                Remove(fragment);
                promiseTasks.Add(fragment.Destroy(false));
            }
            await PromiseTask.WhenAll(promiseTasks);

            for (int index = 0; index < _fragments.Count; index++)
            {
                var fragment = _fragments[index];
                fragment.EndRemove();
            }
        }

        public Fragment Create(IFragmentKey fragmentKey, Activity activity, Fragment parentFragment)
        {
            foreach (var fragment in _fragments)
            {
                if (fragment.ID == fragmentKey.ID)
                {
                    return fragment;
                }
            }

            var instance = Activator.CreateInstance(fragmentKey.FragmentType) as Fragment;
            var uiView = Activator.CreateInstance(fragmentKey.ViewType) as UIView;
            instance.Init(fragmentKey.ID, uiView, activity, parentFragment);
            return instance;
        }
    
        #endregion

        public Fragment FindFragment(IFragmentKey fragmentKey)
        {
            foreach (var fragment in _fragments)
            {
                if (fragment.ID == fragmentKey.ID)
                {
                    return fragment;
                }
            }
            return null;
        }
    }
}
