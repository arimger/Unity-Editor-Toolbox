using UnityEditor;

namespace Toolbox.Editor
{
    public class ToolboxAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (ToolboxManager.Settings &&
                ToolboxManager.SettingsGuid == AssetDatabase.AssetPathToGUID(assetPath))
            {
                ToolboxManager.InitializeSettings(null);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}