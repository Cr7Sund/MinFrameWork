using Cr7Sund.Server.Impl;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorGameLogic : Cr7Sund.GameLogic.GameLogic
    {
        protected override GameBuilder CreateBuilder()
        {
            return new EditorGameBuilder();
        }
    }
}
