using Cr7Sund.EventBus;

namespace Cr7Sund.Server.Impl
{
    public class AddSceneBeginEvent : EventData
    {
        public SceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class AddSceneEndEvent : EventData
    {
        public SceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }
    public class RemoveSceneBeginEvent : EventData
    {
        public SceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class RemoveSceneEndEvent : EventData
    {
        public SceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class SwitchSceneEvent : EventData
    {
        public SceneKey LastScene { get; internal set; }
        public SceneKey CurScene { get; internal set; }
        public override void Clear()
        {
            LastScene = null;
            CurScene = null;
        }
    }
}
