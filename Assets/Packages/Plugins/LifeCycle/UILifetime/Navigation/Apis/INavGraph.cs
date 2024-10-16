using System;
using System.Collections.Generic;
using Cr7Sund.UILifeTime;
namespace Cr7Sund.Navigation
{
    public interface INavGraph
    {
        List<IFragmentKey>  FragmentTypes { get; }
        string Name
        {
            get;
        }
        IList<Fragment> RouteFragments
        {
            get;
        }
    }
}
