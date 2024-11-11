
using Cr7Sund.Package.Api;
using Cr7Sund.Server.UI.Impl;
using UnityEngine;

namespace Cr7Sund.PackageTest.IOC
{
    public class SampleThreePanel : UIView
    {
        public static bool Rejected;
        public static IPromise LoadPromise;


        public override async PromiseTask OnLoad(GameObject go)
        {
            await base.OnLoad(go);
            if (Rejected)
            {
                throw new System.Exception("hello exception");
            }
            else
            {
                await LoadPromise.Join();
            }
        }
    }
}