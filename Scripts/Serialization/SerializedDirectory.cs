using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine
{
    [Serializable]
    public class SerializedDirectory : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField]
        private DefaultAsset directoryAsset;
#endif
        [SerializeField, Disable]
        private string path;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            path = directoryAsset ? AssetDatabase.GetAssetPath(directoryAsset) : null;
#endif
        }

#if UNITY_EDITOR
        public DefaultAsset DirectoryAsset
        {
            get => directoryAsset;
            set => directoryAsset = value;
        }
#endif
        public string Path
        {
            get => path;
            set => path = value;
        }
    }
}