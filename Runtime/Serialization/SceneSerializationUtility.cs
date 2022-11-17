using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Toolbox.Serialization
{
    internal static class SceneSerializationUtility
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

        private static void ConfirmCache()
        {
            //NOTE: refresh data only if the cache is empty,
            //it probably means that it's our first time when we are updating it
            if (cachedScenes.Count == 0)
            {
                RefreshCache();
            }
        }

        private static void RefreshCache()
        {
            cachedScenes.Clear();
            var buildIndex = -1;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                buildIndex++;
                var sceneIndex = scene.enabled ? buildIndex : InvalidSceneIndex;
                var sceneAsset = EditorGUIUtility.Load(scene.path) as SceneAsset;
                if (sceneAsset != null)
                {
                    if (cachedScenes.ContainsKey(sceneAsset))
                    {
                        continue;
                    }

                    cachedScenes.Add(sceneAsset, new SceneData()
                    {
                        BuildIndex = buildIndex,
                        SceneName = sceneAsset.name,
                        ScenePath = scene.path
                    });
                }
            }
        }


        public static bool TryGetSceneData(SceneAsset sceneAsset, out SceneData data)
        {
            ConfirmCache();
            if (!sceneAsset || !cachedScenes.TryGetValue(sceneAsset, out data))
            {
                data = null;
                return false;
            }

            return true;
        }
#endif


        public static int InvalidSceneIndex => -1;
    }
}