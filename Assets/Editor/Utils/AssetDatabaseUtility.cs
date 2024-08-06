namespace Cr7Sund.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;

    /// <summary>
    /// Utilities for Unity's built in AssetDatabase class
    /// Ref from https://github.com/edwardrowe/unity-custom-tool-example/blob/44e6927c63be8db3ab5b50d30a579e677b6c00ba/Assets/PrimativeCreator/Editor/AssetDatabaseUtility.cs
    /// </summary>
    public static class AssetDatabaseUtility
    {
        public const char UnityDirectorySeparator = '/';
        public const string ResourcesFolderName = "Resources";

        /// <summary>
        /// Creates the asset and any directories that are missing along its path.
        /// </summary>
        /// <param name="unityObject">UnityObject to create an asset for.</param>
        /// <param name="unityFilePath">Unity file path (e.g. "Assets/Resources/MyFile.asset".</param>
        public static void CreateAssetAndDirectories(UnityEngine.Object unityObject, string unityFilePath)
        {
            AssetDatabaseUtility.CreateDirectoriesInPath(unityFilePath);

            AssetDatabase.CreateAsset(unityObject, unityFilePath);
        }

        private static void CreateDirectoriesInPath(string unityDirectoryPath)
        {
            var folders = unityDirectoryPath.Split(UnityDirectorySeparator);

            // Error if path does NOT start from Assets
            if (folders.Length > 0 && folders[0] != "Assets")
            {
                var exceptionMessage = "AssetDatabaseUtility CreateDirectoriesInPath expects full Unity path, including 'Assets\\\". " +
                                       "Adding Assets to path.";
                throw new UnityException(exceptionMessage);
            }

            string pathToFolder = string.Empty;
            for (int i = 0; i < folders.Length - 1; i++)
            {
                string folder = folders[i];
                // Don't check for or create empty folders
                if (string.IsNullOrEmpty(folder))
                {
                    continue;
                }

                // Create folders that don't exist
                var newPathToFolder = string.Concat(pathToFolder, UnityDirectorySeparator);
                newPathToFolder = string.Concat(pathToFolder, folder);
                if (!UnityEditor.AssetDatabase.IsValidFolder(newPathToFolder))
                {
                    AssetDatabase.CreateFolder(pathToFolder, folder);
                    AssetDatabase.Refresh();
                }

                pathToFolder = newPathToFolder;
            }
        }
    }
}