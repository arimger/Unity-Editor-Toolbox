using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PrefabReferenceAttribute))]
    public class PrefabReferenceAttributeDrawer : ObjectValidationDrawer
    {
        protected override string GetWarningMessage()
        {
            return "Assigned object has to be a prefab.";
        }

        protected override bool IsObjectValid(Object objectValue, SerializedProperty property)
        {
            return PrefabUtility.GetPrefabAssetType(objectValue) != PrefabAssetType.NotAPrefab;
        }
    }
}