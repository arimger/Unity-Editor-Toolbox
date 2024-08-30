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

#if UNITY_EDITOR
        internal static bool IsAssetValid(DefaultAsset asset, out string path)
        {
            path = asset != null ? AssetDatabase.GetAssetPath(asset) : null;
            if (path == null)
            {
                return true;
            }

            //NOTE: assume that directory can't have any extension
            var extension = System.IO.Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
            {
                return true;
            }

            return false;
        }
#endif
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (IsAssetValid(directoryAsset, out var assetPath))
            {
                DirectoryPath = assetPath;
                return;
            }

            DirectoryAsset = null;
            DirectoryPath = null;
#endif
        }

#if UNITY_EDITOR
        public DefaultAsset DirectoryAsset
        {
            get => directoryAsset;
            set => directoryAsset = value;
        }
#endif
        public string DirectoryPath
        {
            get => path;
            set => path = value;
        }

        [Obsolete("Use " + nameof(DirectoryPath) + " instead.")]
        public string Path
        {
            get => path;
            set => path = value;
        }
    }
}