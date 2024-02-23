using System;
using Cr7Sund.Server.UI.Api;

namespace Cr7Sund.Server.UI.Impl
{
    public class UIKey : IAssetKey
    {
        private readonly Type _ctrlType;
        private readonly Type _viewType;

        public readonly object Intent;
        public readonly bool PlayAnimation;
        public readonly bool Stack;
        public readonly bool LoadAsync;
        public readonly bool HideFirst;


        public IAssetKey exitPage { get; set; }
        public string Key { get; set; }
        public bool IsPush { get; internal set; }
        public bool ShowAfter { get; set; }


        public UIKey(string key, Type ctrlType, Type viewType,
             object intent = null, bool playAnimation = false, bool stack = true, bool loadAsync = true, bool hideFirst = false)
        {
            Key = key;
            _ctrlType = ctrlType;
            _viewType = viewType;

            Intent = intent;
            PlayAnimation = playAnimation;
            Stack = stack;
            LoadAsync = loadAsync;
            HideFirst = hideFirst;
        }

        public IUIController CreateCtrl() => Activator.CreateInstance(_ctrlType) as IUIController;
        public IUIView CreateView() => Activator.CreateInstance(_viewType) as IUIView;

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}