using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(NotPrefabObjectOnlyAttribute))]
    public class NotPrefabObjectOnlyAttributeDrawer : ObjectValidationDrawer
    {
        protected override string GetWarningMessage()
        {
            return "Assigned object can't be a Prefab.";
        }

        protected override bool IsObjectValid(Object objectValue, SerializedProperty property)
        {
            if (objectValue == null)
            {
                return false;
            }

            var attribute = Attribute;
            if (PrefabUtility.GetPrefabAssetType(objectValue) == PrefabAssetType.NotAPrefab)
            {
                return true;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(objectValue) == PrefabInstanceStatus.Connected &&
                attribute.AllowInstancedPrefabs)
            {
                return true;
            }

            return false;
        }

        private NotPrefabObjectOnlyAttribute Attribute => attribute as NotPrefabObjectOnlyAttribute;
    }
}