namespace Cr7Sund.LifeTime
{
    public interface ITransaction
    {
        void StartTransition(string relativeUI, IRouteArgs intent);
        void AddRoute(IRouteKey route);
        void RemoveRoute(IRouteKey route);
        PromiseTask Commit(bool parallelAnimation );
        PromiseTask PopBack(bool parallelAnimation );
    }

    // public class AddressableKeys
    // {
    //     public const string ShopPanel = "";
    //     public const string HomePanel = "";
    // }
    //
    // public class HomeScreen : Fragment
    // {
    //
    // }
    // public class GraphKeys
    // {
    //     public static readonly IRouteKey HomeKey = new RouteKey("HomePanel");
    //     public static readonly IRouteKey ShopKey = new RouteKey(AddressableKeys.ShopPanel,  typeof(HomeScreen));
    //     public static readonly IRouteKey DetailKey = new RouteKey(AddressableKeys.ShopPanel, typeof(HomeScreen))
    //     {
    //         IsDialog = true
    //     };
    //
    //
    //     public static readonly IRouteKey ShopGraphKey = new RouteGraph("ShopGraph", new[]
    //     {
    //         ShopKey
    //     });
    //
    //     public static readonly IRouteKey HomeGraphKey = new RouteGraph("HomeGraph", new[]
    //     {
    //         HomeKey, DetailKey,
    //         ShopGraphKey
    //     });
    // }
    //
    //
}
