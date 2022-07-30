using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeepFreeze.Packages.Toolbox.Editor
{
    public static class AssetDatabaseUtility
    {
        /// <summary>
        /// Returns the asset's path as a directory
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetAssetPathDirectory(UnityEngine.Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            return GetDirectoryFromAssetPath(path);
        }

        /// <summary>
        /// Returns the directory for the specific asset path
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetDirectoryFromAssetPath(string assetPath)
        {
            var directory = string.Concat(System.IO.Path.GetDirectoryName(assetPath),
                System.IO.Path.DirectorySeparatorChar.ToString());
            return directory;
        }

        /// <summary>
        /// Returns the asset path, with the name of subassets appended, if this is a subasset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string GetAssetPathWithSubAsset(UnityEngine.Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            if (AssetDatabase.IsSubAsset(asset))
            {
                path = string.Concat(path, "/", asset.name);
            }

            return path;
        }

        /// <summary>
        /// Loads all assets at the specified directory.
        /// </summary>
        /// <returns>The assets at the specified asset relative directory.</returns>
        /// <param name="assetDirectory">Asset relative directory.</param>
        public static List<Object> LoadAssetsAtDirectory(string assetDirectory)
        {
            // Note this has to use AssetDatabase.Load so that we load sprites and other sub assets and
            // include them as "assets" in the directory. System.IO would not find these files.
            var assetGUIDsInDirectory = AssetDatabase.FindAssets(string.Empty, new string[] { assetDirectory.Substring(0, assetDirectory.Length - 1) });
            var filePaths = new List<string>(assetGUIDsInDirectory.Length);
            for (int i = 0; i < assetGUIDsInDirectory.Length; ++i)
            {
                var guid = assetGUIDsInDirectory[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Only need to add one of each path, because FindAssets returns the same guid multiple times
                // for sub assets (WTH Unity??)
                if (!filePaths.Contains(path))
                {
                    filePaths.Add(path);
                }
            }

            var assetsAtPath = new List<UnityEngine.Object>();
            foreach (var filePath in filePaths)
            {
                // Textures have sprites in them. Add all assets in this file, including the file itself.
                var assetRelativePath = filePath.Substring(filePath.IndexOf("Assets/"));

                // Workaround: Scene assets for some reason error if you load them via LoadAllAssets.
                // (does it maybe try to load the contents inside the scene?)
                if (System.IO.Path.GetExtension(assetRelativePath) == ".unity")
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetRelativePath);
                    assetsAtPath.Add(sceneAsset);
                }
                else
                {
                    var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetRelativePath);
                    foreach (var subAsset in subAssets)
                    {
                        // It's possible for user created assets to include nulls...
                        // not sure how, but our AnimTemplates have a Null in them, so others
                        // may as well. Skip them.
                        if (subAsset != null)
                        {
                            // Ignore objects that are hidden in the hierarchy, as
                            // from the user's perspective they aren't there.
                            if ((subAsset.hideFlags & HideFlags.HideInHierarchy) != 0)
                            {
                                continue;
                            }

                            assetsAtPath.Add(subAsset);
                        }
                    }
                }
            }

            return assetsAtPath;
        }

        /// <summary>
        /// Load all assets at a certain directory of a certain type
        /// </summary>
        /// <param name="assetDirectory"></param>
        /// <param name="fileExtension"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAssetsInDirectory<T>(string assetDirectory, string fileExtension = "") where T : Object
        {
            // Note this has to use AssetDatabase.Load so that we load sprites and other sub assets and
            // include them as "assets" in the directory. System.IO would not find these files.
            var assetGUIDsInDirectory = AssetDatabase.FindAssets(string.Empty, new[] { assetDirectory.Substring(0, assetDirectory.Length - 1) });
            var filePaths = new List<string>(assetGUIDsInDirectory.Length);
            for (var i = 0; i < assetGUIDsInDirectory.Length; ++i)
            {
                var guid = assetGUIDsInDirectory[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Only need to add one of each path, because FindAssets returns the same guid multiple times
                if (!filePaths.Contains(path) && path.EndsWith(fileExtension))
                {
                    filePaths.Add(path);
                }
            }

            var assetsAtPath = new List<T>();
            foreach (var filePath in filePaths)
            {
                // Textures have sprites in them. Add all assets in this file, including the file itself.
                var assetRelativePath = filePath[filePath.IndexOf("Assets/", StringComparison.Ordinal)..];

                // Workaround: Scene assets for some reason error if you load them via LoadAllAssets.
                // (does it maybe try to load the contents inside the scene?)
                if (System.IO.Path.GetExtension(assetRelativePath) == ".unity")
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<Object>(assetRelativePath);
                    assetsAtPath.Add((T)sceneAsset);
                }
                else
                {
                    var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetRelativePath);
                    foreach (var subAsset in subAssets)
                    {
                        // It's possible for user created assets to include nulls...
                        // not sure how, but our AnimTemplates have a Null in them, so others
                        // may as well. Skip them.
                        if (subAsset != null)
                        {
                            // Ignore objects that are hidden in the hierarchy, as
                            // from the user's perspective they aren't there.
                            if ((subAsset.hideFlags & HideFlags.HideInHierarchy) != 0)
                            {
                                continue;
                            }

                            if (subAsset.GetType() == typeof(T))
                            {
                                assetsAtPath.Add((T)subAsset);
                            }
                        }
                    }
                }
            }

            return assetsAtPath;
        }

        /// <summary>
        /// Returns the assets of the given type in the project based on parameters
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="label"></param>
        /// <param name="folder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAssets<T>(string filter = "", string label = "", string folder = null) where T : Object
        {
            var searchQuery = "";
            if (!string.IsNullOrWhiteSpace(filter))
            {
                searchQuery += $"{filter} ";
            }

            if (!string.IsNullOrWhiteSpace(label))
            {
                searchQuery += $"l:{label} ";
            }

            if (folder == null)
            {
                folder = "Assets";
            }
            
            return AssetDatabase.FindAssets($"{searchQuery} t:{typeof(T)}", new []{folder})
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }
    }
}
