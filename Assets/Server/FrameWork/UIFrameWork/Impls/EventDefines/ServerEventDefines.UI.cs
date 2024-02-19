using Cr7Sund.PackageTest.EventBus.Impl;
using Cr7Sund.Server.UI.Impl;

namespace Cr7Sund.Server
{
    public class AddUIBeginEvent : EventData
    {
        public IAssetKey TargetUI { get; internal set; }
        public override void Clear()
        {
            TargetUI = null;
        }
    }

    public class AddUIEndEvent : EventData
    {
        public IAssetKey TargetUI { get; internal set; }
        public override void Clear()
        {
            TargetUI = null;
        }
    }
    public class RemoveUIBeginEvent : EventData
    {
        public IAssetKey TargetUI { get; internal set; }
        public override void Clear()
        {
            TargetUI = null;
        }
    }

    public class RemoveUIEndEvent : EventData
    {
        public IAssetKey TargetUI { get; internal set; }
        public override void Clear()
        {
            TargetUI = null;
        }
    }

    public class SwitchUIEvent : EventData
    {
        public IAssetKey LastUI { get; internal set; }
        public IAssetKey CurUI { get; internal set; }
        public override void Clear()
        {
            LastUI = null;
            CurUI = null;
        }
    }
}
