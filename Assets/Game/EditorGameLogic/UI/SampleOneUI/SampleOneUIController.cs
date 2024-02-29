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
            
            Debug.Debug("Load ui one {StartValue}", StartValue);

            StartValue++;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Debug("Enable ui one");
            _pageContainer.PushPage(SampleUIKeys.SampleTwoUI);
            EnableCount++;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Debug.Debug("Disable ui one");

            EnableCount--;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Debug("Stop ui one");

            StartValue--;
        }
    }
}