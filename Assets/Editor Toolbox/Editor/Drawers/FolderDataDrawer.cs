using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FolderData))]
    public class FolderDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if expanded - draw foldout + 4 properties + additional icon
            return property.isExpanded
                ? EditorGUI.GetPropertyHeight(property, label) + Style.iconHeight - Style.height
                : Style.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //get properties
            var typeProperty = property.FindPropertyRelative("type");
            var nameProperty = property.FindPropertyRelative("name");
            var pathProperty = property.FindPropertyRelative("path");
            //get icons properties
            var usualIconProperty = property.FindPropertyRelative("icon");
            var smallIconProperty = property.FindPropertyRelative("smallIcon");

            var isPathBased = typeProperty.intValue == 0;
            var propertyName = isPathBased 
                ? pathProperty.stringValue 
                : nameProperty.stringValue;

            //begin property
            label = EditorGUI.BeginProperty(position, label, property);
            label.image = isPathBased ? Style.pathBasedIcon : Style.nameBasedIcon;
            label.text = string.IsNullOrEmpty(propertyName) ? label.text : propertyName;
            //set up proper element name
            //if (!string.IsNullOrEmpty(propertyName))
            //{
            //    label.text = "<color=#35354a><size=11>[" + (isPathBased ? "/" : "n") + "]</size></color> " + propertyName;
            //}

            var propertyPosition = new Rect(position.x, position.y, position.width, Style.height);

            if (!(property.isExpanded = EditorGUI.Foldout(propertyPosition, property.isExpanded, label, true, Style.folderLabelStyle)))
            {
                EditorGUI.EndProperty();
                //adjust position for small icon and draw it in label field
                var smallIconRect = new Rect(position.xMax - Style.smallIconWidth, position.yMin - Style.spacing, Style.smallIconWidth, Style.smallIconHeight);

                GUI.DrawTexture(smallIconRect, Style.folderTexture);
                return;
            }

            EditorGUI.indentLevel++;

            var rawPropertyHeight = propertyPosition.height + Style.spacing;
            var sumPropertyHeight = rawPropertyHeight;

            propertyPosition.y += rawPropertyHeight;
            sumPropertyHeight += rawPropertyHeight;
            //draw folder data type property
            EditorGUI.PropertyField(propertyPosition, typeProperty, false);
            propertyPosition.y += rawPropertyHeight;
            sumPropertyHeight += rawPropertyHeight;

            //decide what property should be drawn depenging on folder data type
            if (isPathBased)
            {
                EditorGUI.PropertyField(propertyPosition, pathProperty, false);          
            }
            else
            {
                EditorGUI.PropertyField(propertyPosition, nameProperty, false);
            }

            propertyPosition.y += rawPropertyHeight;
            sumPropertyHeight += rawPropertyHeight;
            //draw normal icon property
            EditorGUI.PropertyField(propertyPosition, usualIconProperty, false);
            propertyPosition.y += rawPropertyHeight;
            sumPropertyHeight += rawPropertyHeight;
            //draw small icon property
            EditorGUI.PropertyField(propertyPosition, smallIconProperty, false);
            position.x += Style.iconsPadding;

            //get specific rects for folder icons
            var usualFolderIconRect = new Rect(position.x, position.yMin + sumPropertyHeight, Style.iconWidth, Style.iconHeight);
            var smallFolderIconRect = new Rect(position.x + Style.iconWidth,
                usualFolderIconRect.y + Style.smallIconHeight / 2 - Style.spacing, Style.smallIconWidth, Style.smallIconHeight);

            //draw folder icons using desired content
            GUI.DrawTexture(usualFolderIconRect, Style.folderTexture, ScaleMode.ScaleToFit);
            GUI.DrawTexture(smallFolderIconRect, Style.folderTexture, ScaleMode.ScaleToFit);

            //handle big icon texture preview if exist
            if (usualIconProperty.objectReferenceValue)
            {
                var previewTexture = usualIconProperty.objectReferenceValue as Texture;

                //adjust big icon rect using predefined style properties
                usualFolderIconRect.x += usualFolderIconRect.width * ToolboxEditorProject.Style.xToWidthRatio;
                usualFolderIconRect.y += usualFolderIconRect.height * ToolboxEditorProject.Style.yToHeightRatio;
                usualFolderIconRect.width = ToolboxEditorProject.Style.iconWidth;
                usualFolderIconRect.height = ToolboxEditorProject.Style.iconHeight;

                //draw big preview icon
                GUI.DrawTexture(usualFolderIconRect, previewTexture, ScaleMode.ScaleToFit);
            }

            //handle small icon texture preview if exist
            if (smallIconProperty.objectReferenceValue)
            {
                var previewTexture = smallIconProperty.objectReferenceValue as Texture;

                //adjust small icon rect using predefined style properties
                smallFolderIconRect.x += ToolboxEditorProject.Style.padding;
                smallFolderIconRect.y += smallFolderIconRect.height * ToolboxEditorProject.Style.yToHeightRatioSmall;
                smallFolderIconRect.width = ToolboxEditorProject.Style.iconWidthSmall;
                smallFolderIconRect.height = ToolboxEditorProject.Style.iconHeightSmall;

                //draw small preview icon
                GUI.DrawTexture(smallFolderIconRect, previewTexture, ScaleMode.ScaleToFit);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal const float iconsPadding = 12.0f;

            internal const float iconWidth = ToolboxEditorProject.Style.folderIconWidth;
            internal const float iconHeight = ToolboxEditorProject.Style.folderIconHeight;
            internal const float smallIconWidth = ToolboxEditorProject.Style.folderIconWidthSmall;
            internal const float smallIconHeight = ToolboxEditorProject.Style.folderIconHeightSmall;

            internal static readonly Texture pathBasedIcon;
            internal static readonly Texture nameBasedIcon;

            internal static readonly Texture2D folderTexture;

            internal static readonly GUIStyle folderLabelStyle;

            static Style()
            {
                folderTexture = EditorGUIUtility.FindTexture("Folder Icon");
                pathBasedIcon = EditorGUIUtility.IconContent("winbtn_win_rest_h")?.image;
                nameBasedIcon = EditorGUIUtility.IconContent("winbtn_win_max_h")?.image;

                folderLabelStyle = new GUIStyle(EditorStyles.foldout)
                {
                    padding = new RectOffset(17, 2, 0, 0),                                                         
                };
            }
        }
    }
}