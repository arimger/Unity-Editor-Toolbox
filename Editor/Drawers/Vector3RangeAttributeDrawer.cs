using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(Vector3RangeAttribute))]
    public class Vector3RangeAttributeDrawer : ToolboxNativePropertyDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (Vector3RangeAttribute)base.attribute;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property);

            if (EditorGUI.EndChangeCheck())
            {
                var vectorData = property.vector3Value;

                vectorData.x = Mathf.Clamp(vectorData.x, attribute.Min, attribute.Max);
                vectorData.y = Mathf.Clamp(vectorData.y, attribute.Min, attribute.Max);
                vectorData.z = Mathf.Clamp(vectorData.z, attribute.Min, attribute.Max);

                property.vector3Value = vectorData;
                property.serializedObject.ApplyModifiedProperties();
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Vector3;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
