using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
public class Vector2RangeAttributeDrawer : PropertyDrawerBase
{
    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        var attribute = (Vector2RangeAttribute)base.attribute;

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);
        if (EditorGUI.EndChangeCheck())
        {
            var vectorData = property.vector2Value;

            vectorData.x = Mathf.Clamp(vectorData.x, attribute.Min, attribute.Max);
            vectorData.y = Mathf.Clamp(vectorData.y, attribute.Min, attribute.Max);

            property.serializedObject.Update();
            property.vector2Value = vectorData;
            property.serializedObject.ApplyModifiedProperties();
        }
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Vector2;
    }
}