using Cr7Sund.LifeTime;

namespace Cr7Sund.Game.UI
{

    public partial class EditorUIKeys
    {
        public static UIKey SampleOneUI = new UIKey(AddressableKeys.SampleOnePanel, typeof(SampleOneUIController));
        public static UIKey SampleTwoUI = new UIKey(AddressableKeys.SampleTwoPanel, typeof(SampleTwoUIController));
        public static UIKey SampleThreeUI = new UIKey(AddressableKeys.SampleThreePanel, typeof(SampleThreeUIController))
        {
            ParallelTransition = false
        };
        public static UIKey SampleFourUI = new UIKey(AddressableKeys.SampleFourPanel, typeof(SampleFourUIController))
        {
            IsInStack = false
        };
    }
}
