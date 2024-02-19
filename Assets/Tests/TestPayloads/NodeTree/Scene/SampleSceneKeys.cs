using Cr7Sund.PackageTest.IOC;
using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public partial class SampleSceneKeys
    {
        public static SceneKey SampleSceneKeyOne =
            new SceneKey("SampleOneScene", typeof(SampleSceneOneBuilder), isVirtualScene: true);

        public static SceneKey SampleSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(SampleSceneTwoBuilder), isVirtualScene: true);
    }
}
