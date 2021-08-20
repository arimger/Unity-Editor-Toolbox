using UnityEditor;
using UnityEngine;
using Toolbox.Editor.Drawers;

[CustomPropertyDrawer(typeof(RandomAttribute))]
public class RandomAttributeDrawer : PropertyDrawerBase
{
    protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeightSafe(property, label);
    }

    protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
    {
        //adjust field width to the random button
        var fieldRect = new Rect(position.x,
                                 position.y,
                                 position.width - Style.buttonWidth - EditorGUIUtility.standardVerticalSpacing,
                                 position.height);
        //set button rect aligned to the field rect
        var buttonRect = new Rect(position.xMax - Style.buttonWidth,
                                  position.y,
                                  Style.buttonWidth,
                                  position.height);

        EditorGUI.PropertyField(fieldRect, property, property.isExpanded);
        //draw random button using the custom style class
        if (GUI.Button(buttonRect, Style.buttonLabel, Style.buttonStyle))
        {
            var random = Random.Range(Attribute.MinValue, Attribute.MaxValue);
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = random;
            }
            else
            {
                property.intValue = (int)random;
            }
        }
    }


    public override bool IsPropertyValid(SerializedProperty property)
    {
        return property.propertyType == SerializedPropertyType.Float ||
               property.propertyType == SerializedPropertyType.Integer;
    }


    private RandomAttribute Attribute => attribute as RandomAttribute;


    private static class Style
    {
        internal static readonly float buttonWidth = 30.0f;

        internal static readonly GUIContent buttonLabel;

        internal static readonly GUIStyle buttonStyle;

        static Style()
        {
            buttonLabel = new GUIContent(" \u211D", "Set random value");
            buttonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 13
            };
        }
    }
}