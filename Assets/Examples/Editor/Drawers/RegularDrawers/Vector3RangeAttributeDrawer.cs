using UnityEngine;
using UnityEditor;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(Vector3RangeAttribute))]
public class Vector3RangeAttributeDrawer : PropertyDrawerBase
{
    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (Vector3RangeAttribute)base.attribute;

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);
        if (EditorGUI.EndChangeCheck())
        {
            var vectorData = property.vector3Value;

            vectorData.x = Mathf.Clamp(vectorData.x, attribute.Min, attribute.Max);
            vectorData.y = Mathf.Clamp(vectorData.y, attribute.Min, attribute.Max);
            vectorData.z = Mathf.Clamp(vectorData.z, attribute.Min, attribute.Max);

            property.serializedObject.Update();
            property.vector3Value = vectorData;
            property.serializedObject.ApplyModifiedProperties();
        }
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Vector3;
    }
}