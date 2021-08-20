using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(Vector3DirectionAttribute))]
public class Vector3DirectionAttributeDrawer : PropertyDrawerBase
{
    private static readonly string[] directionNames = new[]
    {
        "Up (0,1,0)",
        "Down (0,-1,0)",
        "Right (1,0,0)",
        "Left (-1,0,0)",
        "Forward (0,0,1)",
        "Back (0,0,-1)"
    };

    private static readonly Vector3[] directionValues = new[]
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };


    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        var index = ArrayUtility.FindIndex(directionValues, direction => direction == property.vector3Value);

        //begin the true property
        label = EditorGUI.BeginProperty(position, label, property);
        //draw the prefix label
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.BeginChangeCheck();
        //get index value from popup
        index = Mathf.Clamp(index, 0, directionValues.Length - 1);
        index = EditorGUI.Popup(position, index, directionNames);
        index = Mathf.Clamp(index, 0, directionValues.Length - 1);

        if (EditorGUI.EndChangeCheck())
        {
            property.vector3Value = directionValues[index];
        }
        EditorGUI.EndProperty();
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Vector3;
    }
}