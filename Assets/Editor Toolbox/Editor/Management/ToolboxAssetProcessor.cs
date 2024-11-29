using UnityEditor;
using AssetProcessor = UnityEditor.AssetModificationProcessor;

namespace Toolbox.Editor.Management
{
    public class ToolboxAssetProcessor : AssetProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (ToolboxManager.Settings &&
                ToolboxManager.SettingsGuid == AssetDatabase.AssetPathToGUID(assetPath))
            {
                ToolboxManager.TryInitializeSettings(null);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}