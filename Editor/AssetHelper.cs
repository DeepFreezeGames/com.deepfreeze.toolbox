using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    /// <summary>
    /// Utility class for Editor asset operations
    /// </summary>
    public static class AssetHelper
    {
        /// <summary>
        /// Returns the global path of the root of the project
        /// </summary>
        public static string ProjectRootPath => Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);

        /// <summary>
        /// Returns the global path of the ProjectSettings directory in the project
        /// </summary>
        public static string ProjectSettingsRootPath => Path.Combine(ProjectRootPath, "ProjectSettings");
        
        /// <summary>
        /// Convert global path to relative
        /// </summary>
        public static string GlobalPathToRelative(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                return $"Assets{path.Substring(Application.dataPath.Length)}";
            }
            
            Debug.LogError($"Incorrect path.\n{path} does not contain {Application.dataPath}");
            return null;
        }

        /// <summary>
        /// Convert relative path to global
        /// </summary>
        public static string RelativePathToGlobal(string path)
        {
            return Path.Combine(Application.dataPath, path);
        }

        /// <summary>
        /// Convert path from unix style to windows
        /// </summary>
        public static string UnixToWindowsPath(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// Convert path from windows style to unix
        /// </summary>
        public static string WindowsToUnixPath(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// Creates the given object at the given location and returns the created object
        /// </summary>
        public static T CreateAsset<T>(T memoryObject, string localPath) where T : Object
        {
            //Validate given path
            if (string.IsNullOrEmpty(localPath))
            {
                Debug.LogError("Local path is empty");
                return null;
            }
            
            //Standardize path
            localPath = WindowsToUnixPath(localPath);
            
            //Check folder path
            var folders = localPath.Split('/').ToList();
            
            //Remove the file name
            folders.RemoveAt(folders.Count - 1);

            //Generate missing folders
            if (folders.Count > 0)
            {
                var compoundPath = string.Empty;
                for (var i = 0; i < folders.Count; i++)
                {
                    if (!AssetDatabase.IsValidFolder(Path.Combine(compoundPath, folders[i])))
                    {
                        AssetDatabase.CreateFolder(compoundPath, folders[i]);
                    }

                    compoundPath = Path.Combine(compoundPath, folders[i]);
                    AssetDatabase.Refresh();
                }
            }
            
            //Save asset
            AssetDatabase.CreateAsset(memoryObject, localPath);
            
            AssetDatabase.Refresh();

            var finalAsset = AssetDatabase.LoadAssetAtPath<T>(localPath);
            if (finalAsset == null)
            {
                Debug.LogError($"Failed to create asset ({typeof(T)}) at {localPath}");
            }
            
            return finalAsset;
        }

        /// <summary>
        /// Returns all the assets of the given type. Optionally, directories can be given and the search will only
        /// occur within those directories
        /// </summary>
        public static List<T> GetAllAssets<T>(string[] folders = null) where T : Object
        {
            var output = new List<T>();

            var guids = folders != null
                ? AssetDatabase.FindAssets($"t:{typeof(T).Name}", folders)
                : AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    output.Add(asset);
                }
            }
            
            return output;
        }

        public static List<T> GetAllAssets<T>(Type type, string[] folders = null) where T : Object
        {
            var output = new List<T>();
            var guids = folders != null
                ? AssetDatabase.FindAssets($"t:{type.Name}", folders)
                : AssetDatabase.FindAssets($"t:{type.Name}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var asset = AssetDatabase.LoadAssetAtPath(path, type);
                if (asset != null && asset is T typedAsset)
                {
                    output.Add(typedAsset);
                }
            }

            return output;
        }

        public static bool IsResourceAsset(Object obj, out string resourcePath)
        {
            resourcePath = string.Empty;
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (!assetPath.StartsWith("Assets/Resources/"))
            {
                return false;
            }

            assetPath = assetPath.Replace("Assets/Resources/", "");
            var extension = Path.GetExtension(assetPath);
            resourcePath = assetPath.Remove(assetPath.Length - extension.Length, extension.Length);

            return true;
        }
    }
}