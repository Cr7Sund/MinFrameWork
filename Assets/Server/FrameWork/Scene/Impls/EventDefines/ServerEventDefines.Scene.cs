﻿using Cr7Sund.EventBus.Impl;
using Cr7Sund.Server.Apis;
using Cr7Sund.Server.Impl;

namespace Cr7Sund.Server
{
    public class AddSceneBeginEvent : EventData
    {
        public ISceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class AddSceneEndEvent : EventData
    {
        public ISceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }
    public class RemoveSceneBeginEvent : EventData
    {
        public ISceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class RemoveSceneEndEvent : EventData
    {
        public ISceneKey TargetScene { get; internal set; }
        public override void Clear()
        {
            TargetScene = null;
        }
    }

    public class SwitchSceneEvent : EventData
    {
        public ISceneKey LastScene { get; internal set; }
        public ISceneKey CurScene { get; internal set; }
        public override void Clear()
        {
            LastScene = null;
            CurScene = null;
        }
    }
}