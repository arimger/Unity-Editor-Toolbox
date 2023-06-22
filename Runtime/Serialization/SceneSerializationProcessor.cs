using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Toolbox.Serialization
{
#if UNITY_EDITOR
    public class SceneSerializationProcessor : AssetPostprocessor
    {
#if UNITY_2021_3_OR_NEWER
        internal static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
#else
        internal static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
#endif
        {
            foreach (string path in importedAssets)
            {
                if (IsSceneAsset(path))
                {
                    SceneSerializationUtility.RefreshCache();
                    return;
                }
            }

            foreach (string path in deletedAssets)
            {
                if (IsSceneAsset(path))
                {
                    SceneSerializationUtility.RefreshCache();
                    return;
                }
            }
        }

        private static bool IsSceneAsset(string path)
        {
            return Path.GetExtension(path) == ".unity";
        }
    }
#endif
}