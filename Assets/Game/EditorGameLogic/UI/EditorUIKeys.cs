using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Game.UI
{
        public partial class EditorUIKeys
        {
                public static UIKey SampleOneUI = new UIKey("SampleOnePanel",
                        typeof(SampleOneUIController), typeof(SampleOnePanel));
                public static UIKey SampleTwoUI = new UIKey("SampleTwoPanel",
                        typeof(SampleTwoUIController), typeof(SampleTwoPanel), uniqueInstance: false);
                public static UIKey SampleThreeUI = new UIKey("SampleThreePanel",
                      typeof(SampleThreeUIController), typeof(SampleThreePanel), hideFirst: true);
                public static UIKey SampleFourUI = new UIKey("SampleFourPanel",
                                typeof(SampleFourUIController), typeof(SampleFourPanel), stack: false);
        }
}