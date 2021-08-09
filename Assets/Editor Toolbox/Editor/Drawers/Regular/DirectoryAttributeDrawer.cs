using System.IO;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DirectoryAttribute))]
    public class DirectoryAttributeDrawer : PropertyDrawerBase
    {
        private static bool IsPathValid(string propertyPath, string assetRelativePath)
        {
            var targetPath = string.IsNullOrEmpty(assetRelativePath)
                ? Path.Combine(Application.dataPath, propertyPath)
                : Path.Combine(Application.dataPath, assetRelativePath, propertyPath);

            return Directory.Exists(targetPath);
        }

        private Rect DrawWarningMessage(Rect position)
        {
            position = new Rect(position.x,
                                position.y,
                                position.width, Style.boxHeight);
            EditorGUI.HelpBox(position, "Provided directory does not exist.", MessageType.Warning);
            return position;
        }

        private void UseDirectoryPicker(SerializedProperty property, string relativePath)
        {
            var baseDataPath = Application.dataPath;
            var baseOpenPath = Path.GetFileName(baseDataPath);
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
                position = DrawWarningMessage(position);
                position.yMin = position.yMax + Style.spacing;
                position.yMax = position.yMin + Style.rowHeight;
            }

            position.xMax -= Style.pickerWidth + Style.spacing;
            EditorGUI.PropertyField(position, property, label);
            position.xMax += Style.pickerWidth + Style.spacing;
            position.xMin = position.xMax - Style.pickerWidth;

            //create additional pick directory button
            if (GUI.Button(position, Style.pickerContent, EditorStyles.miniButton))
            {
                UseDirectoryPicker(property, Attribute.RelativePath);
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
            internal static readonly float pickerWidth = 30.0f;

            internal static readonly GUIContent pickerContent;

            static Style()
            {
                pickerContent = EditorGUIUtility.IconContent("Folder Icon");
                pickerContent.tooltip = "Pick directory";
            }
        }
    }
}