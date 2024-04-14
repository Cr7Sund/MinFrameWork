using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using Cr7Sund.Server.UI.Api;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleFivePanel : UIView
    {
        public static IPromise AnimPromise ;
        public static bool Rejected;

        public override PromiseTask EnterRoutine(bool push, IUINode partnerPage, bool playAnimation)
        {
            if (Rejected)
            {
                throw new System.Exception("hello exception");
            }
            else
            {
                return AnimPromise.AsNewTask();
            }
        }



    }
}