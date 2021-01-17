using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class RegexValueAttributeDrawer : ToolboxSelfPropertyDrawer<RegexValueAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, RegexValueAttribute attribute)
        {
            EditorGUILayout.PropertyField(property, label);
            if (Regex.IsMatch(property.stringValue, attribute.Pattern))
            {
                return;
            }

            EditorGUILayout.HelpBox(!attribute.HasMessage
                ? "String value does not match the pattern!"
                : attribute.Message, MessageType.Error);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }
    }
}