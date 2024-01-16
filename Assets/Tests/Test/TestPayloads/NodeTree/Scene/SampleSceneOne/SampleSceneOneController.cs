using Cr7Sund.NodeTree.Impl;

namespace Cr7Sund.Framework.Tests
{
    public class SampleSceneOneController : BaseController
    {
        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("Load scene");
        }
    }
}