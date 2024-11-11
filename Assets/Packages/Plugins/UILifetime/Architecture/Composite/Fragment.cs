namespace Cr7Sund.LifeTime
{

    public abstract class Fragment : Node
    {
        [Inject]
        protected NavHost _navHost;
        [Inject(ServerBindDefine.GraphLogger)] protected IInternalLog Debug;

        

        protected NavController FindNavController()
        {
            return _navHost.FindNavController((IRouteKey)Key);
        }
    }

    public abstract class UpdateFragment : UpdatableNode
    {
        [Inject]
        protected NavHost _navHost;
        [Inject(ServerBindDefine.GraphLogger)] protected IInternalLog Debug;


        protected NavController FindNavController()
        {
            return _navHost.FindNavController((IRouteKey)Key);
        }
    }
}
