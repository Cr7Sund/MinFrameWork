using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
        public class SampleUIKeys
        {
                public static UIKey SampleOneUI = new UIKey("SampleOneUI",
                        typeof(SampleOneUIController), typeof(SampleOnePanel));
                public static UIKey SampleTwoUI = new UIKey("SampleTwoUI",
                        typeof(SampleTwoUIController), typeof(SampleTwoPanel));
                public static UIKey SampleThreeUI = new UIKey("SampleThreeUI",
                      typeof(SampleThreeUIController), typeof(SampleThreePanel), hideFirst: true);

                public static UIKey SampleFourUI = new UIKey("SampleFourUI",
                                typeof(SampleFourUIController), typeof(SampleFourPanel), stack: false);
        }
}