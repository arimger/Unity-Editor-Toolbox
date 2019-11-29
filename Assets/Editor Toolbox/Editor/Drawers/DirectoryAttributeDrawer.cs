using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DirectoryAttribute))]
    public class DirectoryAttributeDrawer : ToolboxNativeDrawerBase
    {
        private static bool IsPathValid(string propertyPath, string assetRelativePath)
        {
            var projectRelativePath = Application.dataPath + "/";

            if (!string.IsNullOrEmpty(assetRelativePath))
            {
                projectRelativePath += assetRelativePath + "/";
            }

            return Directory.Exists(projectRelativePath + propertyPath);
        }


        protected override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //validate property type
            if (property.propertyType != SerializedPropertyType.String)
            {
                return base.GetPropertyHeight(property, label);
            }

            var additionalHeight = 0.0f;

            //validate directory path
            if (!IsPathValid(property.stringValue, Attribute.RelativePath))
            {
                additionalHeight = Style.boxHeight + Style.spacing;
            }

            //return proper height
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            //current stored path validation
            if (!IsPathValid(property.stringValue, Attribute.RelativePath))
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, "Provided directory does not exist.", MessageType.Warning);
                position.y += Style.boxHeight + Style.spacing + Style.spacing;
            }

            position.height = Style.height;
            position.width -= Style.directoryButtonWidth + Style.spacing;
            //draw standard string property field
            EditorGUI.PropertyField(position, property, label);
            position.x = position.xMax + Style.spacing;
            position.width = Style.directoryButtonWidth;
            //create additional pick directory button
            if (GUI.Button(position, Style.directoryButtonContent, Style.directoryButtonStyle))
            {
                var assetRelativePath = Attribute.RelativePath;
                var projectRelativePath = Application.dataPath + "/";

                if (!string.IsNullOrEmpty(assetRelativePath))
                {
                    projectRelativePath += assetRelativePath + "/";
                }

                property.stringValue = EditorUtility.OpenFolderPanel("Pick directory", "Assets/" + assetRelativePath, "").Replace(projectRelativePath, "");
            }
        }


        private DirectoryAttribute Attribute => attribute as DirectoryAttribute;


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