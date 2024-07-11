using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Editor
{
    public class GUIDPeekTools
    {
        [MenuItem("Assets/MyMenu/PeekAssetPath")]
        public static string PeekAssetPath()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            EditorGUIUtility.systemCopyBuffer = assetPath;
            Debug.Log($"AssetPath: {assetPath}");
            return assetPath;
        }

        [MenuItem("Assets/MyMenu/PeekGUID")]
        public static string PeekGUID()
        {
            string guid = AssetDatabase.AssetPathToGUID(PeekAssetPath());
            EditorGUIUtility.systemCopyBuffer = guid;
            Debug.Log($"GUID: {guid}");
            return guid;
        }
    }
}