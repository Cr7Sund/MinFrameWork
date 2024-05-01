using System;
using Cr7Sund.Selector.Apis;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.GameLogic
{
    public abstract class GameLogic : IGameLogic
    {
        protected GameNode _gameNode;


        public void Init()
        {
            GameBuilder builder = CreateBuilder();
            _gameNode = GameDirector.Construct(builder);

            OnInit();
        }

        public async PromiseTask Run()
        {
            await _gameNode.Run();
        }

        public void Update(int millisecond)
        {
            _gameNode?.Update(millisecond);
        }

        public void LateUpdate(int millisecond)
        {
            _gameNode?.LateUpdate(millisecond);
        }

        public async PromiseTask DestroyAsync()
        {
            if (_gameNode != null)
            {
                await _gameNode.DestroyAsync();
            }
        }

        protected virtual void OnInit()
        {
        }

        protected abstract GameBuilder CreateBuilder();

    }
}
