using System;
using System.Collections.Generic;
using Cr7Sund.Navigation;
namespace Cr7Sund.UILifeTime
{
    public class FragmentKey<TFragment, TView> : IFragmentKey
        where TFragment : Fragment
        where TView : UIView
    {
        public Type FragmentType => typeof(TFragment);
        public Type ViewType => typeof(TView);

        private readonly Guid _guid = Guid.NewGuid();
        public Guid ID
        {
            get
            {
                return _guid;
            }
        }

        private FragmentKey()
        {
        }

        public static IFragmentKey Create()
        {
            if (!_fragmentKeys.TryGetValue(typeof(TFragment), out var instance))
            {
                instance = new FragmentKey<TFragment, TView>();
                _fragmentKeys.Add(typeof(TFragment), instance);
            }
            return instance;
        }

        private static Dictionary<Type, IFragmentKey> _fragmentKeys
            = new Dictionary<Type, IFragmentKey>();
    }
    public interface IFragmentKey
    {
        Type FragmentType { get; }
        Type ViewType { get; }
        Guid ID { get; }
    }

    public abstract class UINavGraph : INavGraph
    {
        private List<Fragment> _routeFragments = new();
        public List<IFragmentKey> FragmentTypes
        {
            get;
        }
        public abstract string Name
        {
            get;
        }
        public IList<Fragment> RouteFragments
        {
            get => _routeFragments;
        }

        public UINavGraph()
        {
            FragmentTypes = new();
        }

        // protected void Add<TFragment, TView>()
        //     where TFragment : Fragment
        //     where TView : UIView
        // {
        //     FragmentTypes.Add(new FragmentKey<TFragment, TView>());
        // }

        protected void Add(IFragmentKey fragmentKey)
        {
            FragmentTypes.Add(fragmentKey);
        }
    }
}
