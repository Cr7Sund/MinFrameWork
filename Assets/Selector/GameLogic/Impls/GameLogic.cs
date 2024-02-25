using Cr7Sund.Package.Api;
using Cr7Sund.NodeTree.Api;
using Cr7Sund.Selector.Apis;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.GameLogic
{
    public abstract class GameLogic : IGameLogic
    {
        private GameNode _node;


        public void Init()
        {
            GameBuilder builder = CreateBuilder();
            _node = GameDirector.Construct(builder);

            OnInit();
        }

        public void Start()
        {
            _node?.Run();
        }

        public void Update(int millisecond)
        {
            _node?.Update(millisecond);
        }

        public void LateUpdate(int millisecond)
        {
            _node?.LateUpdate(millisecond);
        }

        public IPromise<INode> Destroy()
        {
            return _node?.Destroy();
        }

        protected virtual void OnInit()
        {
        }

        protected abstract GameBuilder CreateBuilder();

    }
}
