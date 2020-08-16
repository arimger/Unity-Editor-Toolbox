using UnityEditor;

namespace Toolbox.Editor
{
    public class ToolboxAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (ToolboxSettingsUtility.Settings)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                if (ToolboxSettingsUtility.SettingsGuid == guid)
                {
                    ToolboxSettingsUtility.InitializeSettings((string)null);
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}