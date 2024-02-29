using Cr7Sund.Game.UI;
using Cr7Sund.Server.Scene.Impl;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.Scene
{
    public class EditorSceneOneController : BaseSceneController
    {
        [Inject] private PageContainer _pageContainer;
        public static int StartValue;
        public static int EnableCount;

        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }

        protected override void OnStart()
        {
            base.OnStart();
            Debug.Debug("Load scene one");

            StartValue++;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable scene one");

            _pageContainer.PushPage(SampleUIKeys.SampleOneUI);
            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable scene one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop scene one");

            StartValue--;
        }
    }
}