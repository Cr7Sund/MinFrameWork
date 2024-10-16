using Cr7Sund.AssetContainers;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneTwoContext : SceneContext
    {
        protected override string Channel => "SampleSceneTwo";

        protected override void OnMappedBindings()
        {
        }

        protected override void OnUnMappedBindings()
        {
        }
    }
}