using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Toolbox.Serialization
{
    internal static class SceneSerializationUtility
    {
#if UNITY_EDITOR
        private readonly static Dictionary<SceneAsset, SceneData> cachedScenes = new Dictionary<SceneAsset, SceneData>();
        private static bool isInitialized;


        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            UnityEngine.Debug.Log("INIT");
            UpdateAllIndexes();

            EditorBuildSettings.sceneListChanged -= UpdateAllIndexes;
            EditorBuildSettings.sceneListChanged += UpdateAllIndexes;
            isInitialized = true;
        }

        private static void UpdateAllIndexes()
        {
            cachedScenes.Clear();
            UnityEngine.Debug.Log("UPDATE INDEXES");
            var buildIndex = -1;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled)
                {
                    continue;
                }

                buildIndex++;
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                if (sceneAsset != null)
                {
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
            if (!sceneAsset || !cachedScenes.TryGetValue(sceneAsset, out data))
            {
                data = null;
                return false;
            }

            return true;
        }
#endif
    }
}