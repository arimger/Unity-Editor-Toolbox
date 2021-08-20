using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(HexColorAttribute))]
public class HexColorAttributeDrawer : PropertyDrawerBase
{
    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        const string hexFormat = "#{0}";

        //begin the true property
        label = EditorGUI.BeginProperty(position, label, property);
        //draw the prefix label
        position = EditorGUI.PrefixLabel(position, label);

        ColorUtility.TryParseHtmlString(string.Format(hexFormat, property.stringValue), out var color);

        //prepare proper rects for each field
        var labelRect = position;
        labelRect.xMax -= position.width / 2;
        var colorRect = position;
        colorRect.xMin += position.width / 2;

        //draw the color field and the associated label
        color = EditorGUI.ColorField(colorRect, GUIContent.none, color, true, true, false);
        property.stringValue = ColorUtility.ToHtmlStringRGBA(color);
        EditorGUI.SelectableLabel(labelRect, string.Format(hexFormat, property.stringValue));
        EditorGUI.EndProperty();
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.String;
    }
}