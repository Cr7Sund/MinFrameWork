using Cr7Sund.Selector.Apis;
using Cr7Sund.LifeTime;

namespace Cr7Sund.GameLogic
{
    public abstract class GameLogic : IGameLogic
    {
        private GameBuilder _gameBuilder;
        protected abstract IRouteKey routeKey { get; }
        
        public async PromiseTask Run()
        {
            _gameBuilder = new GameBuilder(routeKey);
            await _gameBuilder.LaunchActivity();
            await OnCreate();
        }

        public void Update(int millisecond)
        {
            _gameBuilder.Update(millisecond);
        }

        public void LateUpdate(int millisecond)
        {
            _gameBuilder.LateUpdate(millisecond);
        }

        public PromiseTask DestroyAsync()
        {
            _gameBuilder.CloseActivity();
            return PromiseTask.CompletedTask;
        }

        protected  virtual PromiseTask OnCreate()
        {
            return PromiseTask.CompletedTask;
        }

    }
}
