using Cr7Sund.PackageTest.IOC;
using Cr7Sund.AssetContainers;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public partial class SampleSceneKeys
    {
        public static SceneKey SampleSceneKeyOne =
            new SceneKey("SampleOneScene", typeof(SampleSceneOneBuilder), loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single, isVirtualScene: false);

        public static SceneKey SampleSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(SampleSceneTwoBuilder), loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single, isVirtualScene: false);

        public static SceneKey SampleSceneKeyThree =
            new SceneKey("SampleOneScene", typeof(SampleSceneOneBuilder), loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive, isVirtualScene: false);

        public static SceneKey SampleSceneKeyFour =
            new SceneKey("SampleTwoScene", typeof(SampleSceneTwoBuilder), loadMode: UnityEngine.SceneManagement.LoadSceneMode.Additive, isVirtualScene: false);

    }
}
