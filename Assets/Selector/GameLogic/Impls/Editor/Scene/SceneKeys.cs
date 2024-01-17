using Cr7Sund.Framework.Tests;
using Cr7Sund.Server.Impl;
using UnityEngine.SceneManagement;

namespace Cr7Sund.Framework.Tests
{
    public partial class SceneKeys
    {
        public static SceneKey EditorSceneKeyOne =
            new SceneKey("SampleOneScene", typeof(EditorSceneOneBuilder));

        public static SceneKey EditorSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(EditorSceneTwoBuilder), LoadSceneMode.Additive);
    }
}
