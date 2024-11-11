namespace Cr7Sund.LifeTime
{
    public abstract class Activity : Node
    {
        [Inject]
        protected NavHost _navHost;

        [Inject(ServerBindDefine.ActivityLogger)] protected IInternalLog Debug;

        // PLAN should upwards to output error and exceptions in open panel
   

        
        protected NavController FindNavController()
        {
            return _navHost.FindNavController(Key);
        }
    }

}
