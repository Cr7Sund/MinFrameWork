using Cr7Sund.Game.GameLogic;
using Cr7Sund.LifeTime;
namespace Cr7Sund.Game.Scene
{
    public partial class EditorActivityKeys
    {
        public static SceneKey LoginActivityKey =
            new SceneKey("LoginActivity", typeof(LoginActivity), typeof(LoginContext));

        public static SceneKey HotfixActivityKey =
            new SceneKey("Hotfix", typeof(HotfixActivity), typeof(HotfixContext));

    }
}
