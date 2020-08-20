using UnityEditor;

namespace Toolbox.Editor
{
    public class ToolboxAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (ToolboxManager.Settings)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                if (ToolboxManager.SettingsGuid == guid)
                {
                    ToolboxManager.InitializeSettings((string)null);
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}