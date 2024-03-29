﻿using Cr7Sund.Package.EventBus.Impl;

namespace Cr7Sund.Server.Scene.Impl
{
    public class AddSceneBeginEvent : EventData
    {
        public IAssetKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class AddSceneEndEvent : EventData
    {
        public IAssetKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }
    public class RemoveSceneBeginEvent : EventData
    {
        public IAssetKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class RemoveSceneEndEvent : EventData
    {
        public IAssetKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
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
