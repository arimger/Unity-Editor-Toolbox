using System.IO;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DirectoryAttribute))]
    public class DirectoryAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private static bool IsPathValid(string propertyPath, string assetRelativePath)
        {
            var targetPath = string.IsNullOrEmpty(assetRelativePath)
                ? Path.Combine(Application.dataPath, propertyPath)
                : Path.Combine(Application.dataPath, assetRelativePath, propertyPath);

            return Directory.Exists(targetPath);
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            //validate property type and serialized path
            return IsPathValid(property.stringValue, Attribute.RelativePath) 
                ? base.GetPropertyHeightSafe(property, label) 
                : base.GetPropertyHeightSafe(property, label) + Style.boxHeight + Style.spacing * 2;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //validate currently serialized path value
            if (!IsPathValid(property.stringValue, Attribute.RelativePath))
            {
                var helpBoxRect = new Rect(position.x,
                                           position.y,
                                           position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, "Provided directory does not exist.", MessageType.Warning);
                position.y += Style.boxHeight + Style.spacing + Style.spacing;
            }

            position.height = Style.rowHeight;
            position.width -= Style.directoryButtonWidth + Style.spacing;
            //draw the default string property field
            EditorGUI.PropertyField(position, property, label);
            position.x = position.xMax + Style.spacing;
            position.width = Style.directoryButtonWidth;

            //create additional pick directory button
            if (GUI.Button(position, Style.directoryButtonContent, EditorStyles.miniButton))
            {
                var relativePath = Attribute.RelativePath;
                var baseDataPath = Application.dataPath;
                var baseOpenPath = "Assets";
                if (!string.IsNullOrEmpty(relativePath))
                {
                    baseDataPath = Path.Combine(baseDataPath, relativePath);
                    baseOpenPath = Path.Combine(baseOpenPath, relativePath);
                }

                var selectedPath = EditorUtility.OpenFolderPanel("Pick directory", baseOpenPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    //Unity's API always returns slash
                    baseDataPath = baseDataPath.Replace('\\', '/');
                    selectedPath = selectedPath.Replace(baseDataPath, "");
                    selectedPath = selectedPath.Remove(0, 1);

                    property.serializedObject.Update();
                    property.stringValue = selectedPath;
                    property.serializedObject.ApplyModifiedProperties();
                }

                //NOTE: we have to exit GUI since the EditorUtility.OpenFolderPanel method will break the layouting system
                GUIUtility.ExitGUI();
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        private DirectoryAttribute Attribute => attribute as DirectoryAttribute;


        private static class Style
        {
            internal static readonly float rowHeight = EditorGUIUtility.singleLineHeight;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
#else
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.5f;
#endif
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float directoryButtonWidth = 30.0f;

            internal static readonly GUIContent directoryButtonContent;

            static Style()
            {
                directoryButtonContent = new GUIContent(EditorGUIUtility.FindTexture("Folder Icon"), "Pick directory");
            }
        }
    }
}