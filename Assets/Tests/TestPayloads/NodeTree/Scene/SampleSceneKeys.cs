using Cr7Sund.Framework.Tests;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Framework.Tests
{
    public partial class SampleSceneKeys
    {
        public static SceneKey SampleSceneKeyOne =
            new SceneKey("SampleOneScene", typeof(SampleSceneOneBuilder));

        public static SceneKey SampleSceneKeyTwo =
            new SceneKey("SampleTwoScene", typeof(SampleSceneTwoBuilder));
    }
}
