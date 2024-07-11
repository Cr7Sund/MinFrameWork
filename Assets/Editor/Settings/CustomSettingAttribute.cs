using System;
using UnityEditor;

namespace Cr7Sund.Editor
{
    public class CustomSettingAttribute : Attribute
    {
        public SettingsScope scope;
        public string category;


        public CustomSettingAttribute(SettingsScope scope = SettingsScope.User, string pathPartialToCategory = "")
        {
            this.scope = scope;
        }
    }

}