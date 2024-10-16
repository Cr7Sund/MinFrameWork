using Cr7Sund.AssetContainers;

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
