using System;
using Cr7Sund.LifeTime;

namespace Cr7Sund.LifeTime
{
    public interface ILoadModule : IDisposable
    {
        PromiseTask StartAdd(IRouteKey parentKey, IRouteKey assetKey, IRouteArgs fragmentContext);
        PromiseTask Load(IRouteKey targetKey);
        PromiseTask Preload(IRouteKey assetKey, IRouteArgs fragmentContext);
        PromiseTask Attach(IRouteKey assetKey,IRouteArgs fragmentContext);
        PromiseTask AppendTransition(IRouteKey route);
        PromiseTask Visible(IRouteKey targetKey);
        void EndAdd(IRouteKey assetKey);

        PromiseTask StartRemove(IRouteKey targetKey, bool isRemove);
        PromiseTask Invisible(IRouteKey targetKey);
        PromiseTask DisAttach(IRouteKey assetKey, bool isRemove);
        void EndRemove(IRouteKey targetKey, bool isRemove);

    }
}
