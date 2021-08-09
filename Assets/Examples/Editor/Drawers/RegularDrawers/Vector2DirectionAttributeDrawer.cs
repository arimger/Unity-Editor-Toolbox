using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(Vector2DirectionAttribute))]
public class Vector2DirectionAttributeDrawer : PropertyDrawerBase
{
    private static readonly string[] directionNames = new[]
    {
        "Up (0,1)",
        "Down (0,-1)",
        "Right (1,0)",
        "Left (-1,0)"
    };

    private static readonly Vector2[] directionValues = new[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.right,
        Vector2.left
    };


    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        var index = ArrayUtility.FindIndex(directionValues, direction => direction == property.vector2Value);

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
            property.vector2Value = directionValues[index];
        }
        EditorGUI.EndProperty();
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Vector2;
    }
}