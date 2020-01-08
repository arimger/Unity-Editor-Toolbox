using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PrefabReferenceAttribute))]
    public class PrefabReferenceAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property);

            if (!EditorGUI.EndChangeCheck() || property.objectReferenceValue == null)
            {
                return;
            }

            if (PrefabUtility.GetPrefabAssetType(property.objectReferenceValue) == PrefabAssetType.NotAPrefab)
            {
                LogWarning(property, attribute, "Assigned object has to be a prefab.");
                property.objectReferenceValue = null;
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}
