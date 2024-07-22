using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.Server
{
    public struct AddUIBeginEvent : IEventData
    {
        public IAssetKey TargetUI;
        public IAssetKey ParentUI;
        public string GUID;
    }

    public struct AddUIEndEvent : IEventData
    {
        public IAssetKey TargetUI;
        public IAssetKey ParentUI;
    }

    public struct AddUIFailEvent : IEventData
    {
        public IAssetKey TargetUI;
        public IAssetKey ParentUI;
        public string GUID;
        public bool IsUnload;
    }

    public struct RemoveUIBeginEvent : IEventData
    {
        public IAssetKey TargetUI;
    }

    public struct RemoveUIEndEvent : IEventData
    {
        public IAssetKey ParentUI;
        public IAssetKey TargetUI;
        public bool IsUnload;
    }

    public struct SwitchUIEvent : IEventData
    {
        public IAssetKey LastUI;
        public IAssetKey CurUI;
    }
}
