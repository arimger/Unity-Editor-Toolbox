using System;

using Toolbox.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    /// <summary>
    /// Custom serializable class used to serialize the <see cref="SceneAsset"/> class.
    /// </summary>
    [Serializable]
    public class SerializedScene : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private SceneAsset sceneReference;
#endif
        [SerializeField, HideInInspector]
        private string sceneName;
        [SerializeField, HideInInspector]
        private string scenePath;
        [SerializeField]
        private int buildIndex;


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UpdateProperties();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }


        private void UpdateProperties()
        {
#if UNITY_EDITOR
            if (SceneSerializationUtility.TryGetSceneData(sceneReference, out var sceneData))
            {
                SceneName = sceneData.SceneName;
                ScenePath = sceneData.ScenePath;
                BuildIndex = sceneData.BuildIndex;
            }
            else
            {
                SceneName = string.Empty;
                ScenePath = string.Empty;
                BuildIndex = -1;
            }
#endif
        }

#if UNITY_EDITOR
        public SceneAsset SceneReference
        {
            get => sceneReference;
            set => sceneReference = value;
        }
#endif

        public string SceneName
        {
            get => sceneName;
            set => sceneName = value;
        }

        public string ScenePath
        {
            get => scenePath;
            set => scenePath = value;
        }

        public int BuildIndex
        {
            get => buildIndex;
            set => buildIndex = value;
        }

        /// <summary>
        /// Indicates if serialized <see cref="BuildIndex"/> has appropriate value and can be loaded in runtime.
        /// </summary>
        public bool CanBeLoaded
        {
            get => BuildIndex != SceneSerializationUtility.InvalidSceneIndex;
        }
    }
}