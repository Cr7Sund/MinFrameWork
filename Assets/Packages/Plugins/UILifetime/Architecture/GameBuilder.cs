namespace Cr7Sund.LifeTime
{
    public class GameBuilder
    {
        private INode _rootActivity;


        public  GameBuilder(IRouteKey rootKey)
        {
            _rootActivity = rootKey.CreateNode() ;
        }
        
        public async PromiseTask LaunchActivity()
        {
            await _rootActivity.StartCreate(null, null);
        }

        public void CloseActivity()
        {
            _rootActivity.Destroy();
        }

        
        public void Update(int elapse)
        {
            if (_rootActivity is IUpdatable updatable)
            {
                updatable.Update(elapse);
            }
        }

        public void LateUpdate(int elapse)
        {
            if (_rootActivity is ILateUpdatable updatable)
            {
                updatable.LateUpdate(elapse);
            }
        }

    }
}
