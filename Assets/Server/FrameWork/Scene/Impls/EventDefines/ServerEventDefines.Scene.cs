using Cr7Sund.Package.EventBus.Api;

namespace Cr7Sund.Server.Scene.Impl
{
    public struct AddSceneBeginEvent : IEventData
    {
        public IAssetKey TargetScene;
        public string guid;
    }

    public struct AddSceneEndEvent : IEventData
    {
        public IAssetKey TargetScene;
    }

    public struct AddSceneFailEvent : IEventData
    {
        public IAssetKey TargetScene;
        public string guid;
        public bool IsUnload;
    }
    public struct RemoveSceneBeginEvent : IEventData
    {
        public IAssetKey TargetScene;
    }

    public struct RemoveSceneEndEvent : IEventData
    {
        public bool IsUnload;
        public IAssetKey TargetScene;
    }

    public struct SwitchSceneEvent : IEventData
    {
        public IAssetKey LastScene;
        public IAssetKey CurScene;
    }
}
