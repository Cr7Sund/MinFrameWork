using Cr7Sund.Package.EventBus.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public abstract class BaseSceneEvent : EventData
    {
        public IAssetKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class AddSceneBeginEvent : BaseSceneEvent
    {

    }

    public class AddSceneEndEvent : BaseSceneEvent
    {
    }
    public class RemoveSceneBeginEvent : BaseSceneEvent
    {

    }

    public class RemoveSceneEndEvent : BaseSceneEvent
    {
    }

    public class SwitchSceneEvent : EventData
    {
        public IAssetKey LastScene { get; internal set; }
        public IAssetKey CurScene { get; internal set; }
        public override void Clear()
        {
            LastScene = null;
            CurScene = null;
        }
    }
}
