using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

namespace Toolbox.Serialization
{
    public static class SceneSerializationUtility
    {
#if UNITY_EDITOR
        private static readonly Dictionary<SceneAsset, SceneData> cachedScenes = new Dictionary<SceneAsset, SceneData>();
        private static bool isInitialized;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            ConfirmCache();
            EditorBuildSettings.sceneListChanged -= RefreshCache;
            EditorBuildSettings.sceneListChanged += RefreshCache;
            isInitialized = true;
        }

        private static bool TryCreateSceneData(SceneAsset sceneAsset, out SceneData sceneData)
        {
            var path = AssetDatabase.GetAssetPath(sceneAsset);
            if (string.IsNullOrEmpty(path))
            {
                sceneData = null;
                return false;
            }

            sceneData = new SceneData()
            {
                BuildIndex = SceneUtility.GetBuildIndexByScenePath(path),
                SceneName = sceneAsset.name,
                ScenePath = path,
            };

            return true;
        }

        private static bool CanRefreshCache()
        {
            return !EditorApplication.isUpdating;
        }

        internal static void ConfirmCache()
        {
            //NOTE: refresh data only if the cache is empty,
            //it probably means that it's our first time when we are updating it
            if (cachedScenes.Count == 0)
            {
                RefreshCache();
            }
        }

        internal static void RefreshCache()
        {
            if (!CanRefreshCache())
            {
                return;
            }

            cachedScenes.Clear();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                var path = scene.path;
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var sceneAsset = EditorGUIUtility.Load(path) as SceneAsset;
                if (sceneAsset == null)
                {
                    continue;
                }

                if (cachedScenes.ContainsKey(sceneAsset))
                {
                    continue;
                }

                if (TryCreateSceneData(sceneAsset, out var sceneData))
                {
                    cachedScenes.Add(sceneAsset, sceneData);
                }
            }

            OnCacheRefreshed?.Invoke();
        }

        internal static bool TryGetSceneData(SceneAsset sceneAsset, out SceneData data)
        {
            if (sceneAsset == null)
            {
                data = null;
                return false;
            }

            if (CanRefreshCache())
            {
                ConfirmCache();
                if (cachedScenes.TryGetValue(sceneAsset, out data))
                {
                    return true;
                }
            }
            else
            {
                if (TryCreateSceneData(sceneAsset, out data))
                {
                    return true;
                }
            }

            data = null;
            return false;
        }
#endif
        /// <summary>
        /// Event fired each time Scenes cache is rebuilt. 
        /// Potential triggers:
        /// - Scene Build Settings changed
        /// - Scene asset removed/added
        /// 
        /// You can use it to invalidate/SetDirty all assets that use <see cref="UnityEngine.SerializedScene"/> to validate serialized Scene indexes.
        /// </summary>
        public static event Action OnCacheRefreshed;

        internal static int InvalidSceneIndex => -1;
    }
}