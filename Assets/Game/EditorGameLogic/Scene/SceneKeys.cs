using Cr7Sund.Server.Scene.Impl;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Game.Scene
{
    public partial class SceneKeys
    {
        public static SceneKey EditorSceneKeyOne =
            new SceneKey("SampleOneScene", typeof(EditorSceneOneBuilder));

        public static SceneKey EditorAddictiveSceneKeyOne =
            new SceneKey("SampleTwoScene", typeof(EditorSceneTwoBuilder), LoadSceneMode.Additive);
        public static SceneKey EditorAddictiveSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(EditorSceneTwoBuilder), LoadSceneMode.Additive);

    }
}
