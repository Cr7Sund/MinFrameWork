using Cr7Sund.LifeTime;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Game.Scene
{
    public partial class EditorSceneKeys
    {
        public static SceneKey LoginSceneKey =
            new SceneKey("SampleOneScene", typeof(EditorSceneOneController));


        public static SceneKey EditorAddictiveSceneKeyOne =
            new SceneKey("SampleTwoScene", typeof(EditorSceneTwoController))
            {
                LoadSceneMode = LoadSceneMode.Single
            };
        public static SceneKey EditorAddictiveSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(EditorSceneTwoController))
            {
                LoadSceneMode = LoadSceneMode.Single
            };

    }

}
