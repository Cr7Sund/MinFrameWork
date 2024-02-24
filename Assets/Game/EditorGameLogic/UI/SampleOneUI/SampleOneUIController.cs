using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
    public class SampleOneUIController : BaseUIController
    {
        public static int StartValue;
        public static int EnableCount;

        [Inject] private PageContainer _pageContainer;

        
        public static void Init()
        {
            StartValue = 0;
            EnableCount = 0;
        }


        protected override void OnStart()
        {
            base.OnStart();
            Debug.Info("Load ui one");

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Info("Enable ui one");
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Info("Disable ui one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Info("Stop ui one");

            StartValue--;
        }
    }
}