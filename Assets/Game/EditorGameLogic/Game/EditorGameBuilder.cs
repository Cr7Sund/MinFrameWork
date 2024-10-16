using Cr7Sund.AssetContainers;

namespace Cr7Sund.Game.GameLogic
{
    public class EditorGameBuilder : GameBuilder
    {
        protected override GameContext CreateContext()
        {
            return new EditorGameContext();
        }

        protected override BaseGameController CreateController()
        {
            return new EditorMainController();
        }
    }
}