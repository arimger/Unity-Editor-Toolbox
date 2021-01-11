using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    [Serializable]
    public class SerializedScene : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        private static Dictionary<SceneAsset, int> cachedBuildIndexes = new Dictionary<SceneAsset, int>();

        [SerializeField]
        private SceneAsset sceneReference;
#endif
        [SerializeField, HideInInspector]
        private int buildIndex;


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            TryGetBuildIndex(sceneReference, out buildIndex);
#endif
        }


#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            UpdateAllIndexes();

            EditorBuildSettings.sceneListChanged -= UpdateAllIndexes;
            EditorBuildSettings.sceneListChanged += UpdateAllIndexes;
        }

        private static void UpdateAllIndexes()
        {
            cachedBuildIndexes.Clear();

            var buildIndex = -1;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    buildIndex++;
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                    if (sceneAsset != null)
                    {
                        cachedBuildIndexes.Add(sceneAsset, buildIndex);
                    }
                }
            }
        }

        private static bool TryGetBuildIndex(SceneAsset sceneAsset, out int index)
        {
            if (sceneAsset == null || !cachedBuildIndexes.TryGetValue(sceneAsset, out index))
            {
                index = -1;
                return false;
            }

            return true;
        }


        public SceneAsset SceneReference
        {
            get => sceneReference;
            set => sceneReference = value;
        }
#endif
        public int BuildIndex
        {
            get => buildIndex;
            set => buildIndex = value;
        }
    }
}