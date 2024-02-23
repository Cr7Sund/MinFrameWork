using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Cr7Sund.Editor
{
    public class AddressableHelper
    {
        public static bool IsAssetAddressable(UnityEngine.Object obj)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));
            return entry != null;
        }
    }
}