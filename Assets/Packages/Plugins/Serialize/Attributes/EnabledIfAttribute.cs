using System;
using UnityEngine;


namespace Cr7Sund
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnabledIfAttribute : PropertyAttribute
    {
        public enum HideMode
        {
            Invisible,
            Disabled
        }

        public enum SwitcherType
        {
            Bool,
            Enum
        }

        public int enableIfValueIs;
        public HideMode hideMode;
        public string switcherFieldName;

        public SwitcherType switcherType;

        public EnabledIfAttribute(string switcherFieldName, bool enableIfValueIs, HideMode hideMode = HideMode.Disabled)
            : this(SwitcherType.Bool, switcherFieldName, enableIfValueIs ? 1 : 0, hideMode)
        {
        }

        public EnabledIfAttribute(string switcherFieldName, int enableIfValueIs, HideMode hideMode = HideMode.Disabled)
            : this(SwitcherType.Enum, switcherFieldName, enableIfValueIs, hideMode)
        {
        }

        private EnabledIfAttribute(SwitcherType switcherType, string switcherFieldName, int enableIfValueIs,
            HideMode hideMode)
        {
            this.switcherType = switcherType;
            this.hideMode = hideMode;
            this.switcherFieldName = switcherFieldName;
            this.enableIfValueIs = enableIfValueIs;
        }
    }
}