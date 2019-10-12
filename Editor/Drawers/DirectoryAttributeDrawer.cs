using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(DirectoryAttribute))]
    public class DirectoryAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //validate property type
            if (property.propertyType != SerializedPropertyType.String)
            {
                return base.GetPropertyHeight(property, label);
            }

            var additionalHeight = 0.0f;

            //validate directory path
            if (!Directory.Exists(Application.dataPath + "/" + property.stringValue))
            {
                additionalHeight = Style.boxHeight + Style.spacing;
            }

            //return proper height
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on string value properties.");
                EditorGUI.PropertyField(position, property, label, property.isExpanded);
                return;
            }

            //current stored path validation
            if (!Directory.Exists(Application.dataPath + "/" + property.stringValue))
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, "Provided directory does not exist in Project.", MessageType.Warning);
                position.y += Style.boxHeight + Style.spacing + Style.spacing;
            }

            position.height = Style.height;
            position.width -= Style.directoryButtonWidth + Style.spacing;
            //draw standard string property field
            EditorGUI.PropertyField(position, property, label);
            position.x = position.xMax + Style.spacing; ;
            position.width = Style.directoryButtonWidth;
            //create additional pick directory button
            if (GUI.Button(position, Style.directoryButtonContent, Style.directoryButtonStyle))
            {
                property.stringValue = EditorUtility.OpenFolderPanel("Pick directory", "Assets", "").Replace(Application.dataPath + "/", "");
            }
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.5f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float directoryButtonWidth = 30.0f;

            internal static readonly GUIStyle directoryButtonStyle;

            internal static readonly GUIContent directoryButtonContent;
    
            static Style()
            {
                directoryButtonStyle = new GUIStyle(EditorStyles.miniButton);
                directoryButtonContent = new GUIContent(EditorGUIUtility.FindTexture("Folder Icon"), "Pick directory");
            }
        }
    }
}