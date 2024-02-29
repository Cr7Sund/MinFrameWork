using Cr7Sund.Server.Impl;
using Cr7Sund.Server.Scene.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleSceneOneContext : SceneContext
    {
        protected override string Channel => "SampleSceneOne";

        protected override void OnMappedBindings()
        {
        }

        protected override void OnUnMappedBindings()
        {
        }
    }
}