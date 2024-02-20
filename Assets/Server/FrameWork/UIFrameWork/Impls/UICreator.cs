using System;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.UI.Impl
{
    public static class UICreator
    {
        public static UINode Create(UIKey key)
        {
            return new UINode()
            {
                Key = key,
                PageId = Guid.NewGuid().ToString(),
                View = key.CreateView(),
                Controller = key.CreateCtrl(),
            };
        }
    }
}
