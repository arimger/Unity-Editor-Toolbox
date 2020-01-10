using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FolderData))]
    public class FolderDataDrawer : PropertyDrawer
    {
        private const string selectorEventName = "ObjectSelectorUpdated";

        private const int usualIconPickedId = 1001;
        private const int smallIconPickedId = 1002;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {            
            //if expanded - draw foldout + 3 properties + buttons strip + additional icon 
            if (property.isExpanded)
            {
                var height = Style.height;
                var typeProperty = property.FindPropertyRelative("type");
                var nameProperty = property.FindPropertyRelative("name");
                var pathProperty = property.FindPropertyRelative("path");

                //check if type is path-based or name-based
                if (typeProperty.intValue == 0)
                {
                    height += EditorGUI.GetPropertyHeight(pathProperty);
                }
                else
                {
                    height += EditorGUI.GetPropertyHeight(nameProperty);
                }

                height += Style.spacing * 4;
                height += Style.height;
                height += Style.height;
                height += Style.height;
                height += Style.iconHeight;
                return height;
            }

            return Style.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //get properties
            var typeProperty = property.FindPropertyRelative("type");
            var nameProperty = property.FindPropertyRelative("name");
            var pathProperty = property.FindPropertyRelative("path");
            var toolProperty = property.FindPropertyRelative("tooltip");
            //get icons properties
            var usualIconProperty = property.FindPropertyRelative("icon");
            var smallIconProperty = property.FindPropertyRelative("smallIcon");

            var isPathBased = typeProperty.intValue == 0;
            var propertyName = isPathBased 
                ? pathProperty.stringValue 
                : nameProperty.stringValue;

            //begin property
            label = EditorGUI.BeginProperty(position, label, property);
            label.text = string.IsNullOrEmpty(propertyName) ? label.text : propertyName;

            var propertyPosition = new Rect(position.x, position.y, position.width, Style.height);

            if (!(property.isExpanded = EditorGUI.Foldout(propertyPosition, property.isExpanded, label, true, Style.folderLabelStyle)))
            {
                EditorGUI.EndProperty();

                //adjust position for small icon and draw it in label field
                var typeIconRect = new Rect(position.xMax - Style.smallIconWidth,
                                            position.yMin - Style.spacing, Style.smallIconWidth, Style.smallIconHeight);

                //draw additional type icon on folder icon
                GUI.DrawTexture(typeIconRect, Style.folderTexture);

                if (isPathBased)
                {
                    typeIconRect.y += Style.spacing;
                    typeIconRect.x += Style.spacing * 1.5f;
                    GUI.DrawTexture(typeIconRect, Style.folderTexture);
                }

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

            //decide what property should be drawn depending on folder data type
            if (isPathBased)
            {
                propertyPosition.height = EditorGUI.GetPropertyHeight(pathProperty);
                EditorGUI.PropertyField(propertyPosition, pathProperty, false);
                propertyPosition.y += propertyPosition.height + Style.spacing;
                sumPropertyHeight += propertyPosition.height + Style.spacing;
                propertyPosition.height = Style.height;
            }
            else
            {
                EditorGUI.PropertyField(propertyPosition, nameProperty, false);
                propertyPosition.y += rawPropertyHeight;
                sumPropertyHeight += rawPropertyHeight;
            }

            //draw tooltip property
            EditorGUI.PropertyField(propertyPosition, toolProperty, false);
            propertyPosition.y += rawPropertyHeight;
            sumPropertyHeight += rawPropertyHeight;

            //adjust rect for folder icons + button strip
            propertyPosition.y += Style.spacing;
            propertyPosition.x += Style.iconsPadding;
            propertyPosition.width = Style.iconWidth;

            //draw normal icon property picker
            if (GUI.Button(propertyPosition, Style.usualIconPickerContent, Style.usualIconPickerStyle))
            {
                var propertyHash = property.GetPropertyKey().GetHashCode();
                EditorGUIUtility.ShowObjectPicker<Texture>(usualIconProperty.objectReferenceValue, false, null, propertyHash + usualIconPickedId);
            }

            propertyPosition.x += Style.iconWidth;
            propertyPosition.width = Style.smallIconWidth;

            //draw small icon property picker
            if (GUI.Button(propertyPosition, Style.smallIconPickerContent, Style.smallIconPickerStyle))
            {
                var propertyHash = property.GetPropertyKey().GetHashCode();
                EditorGUIUtility.ShowObjectPicker<Texture>(smallIconProperty.objectReferenceValue, false, null, propertyHash + smallIconPickedId);
            }

            //catch object selection event and assign it to proper property
            if (Event.current.commandName == selectorEventName)
            {
                //get proper action id by removing unique property hash code
                var controlId = EditorGUIUtility.GetObjectPickerControlID() - property.GetPropertyKey().GetHashCode();
                switch (controlId)
                {
                    case usualIconPickedId:
                        usualIconProperty.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        break;
                    case smallIconPickedId:
                        smallIconProperty.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        break;
                }
            }

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

            internal static readonly Texture2D folderTexture;

            internal static readonly GUIStyle folderLabelStyle;
            internal static readonly GUIStyle usualIconPickerStyle;
            internal static readonly GUIStyle smallIconPickerStyle;

            internal static readonly GUIContent usualIconPickerContent;
            internal static readonly GUIContent smallIconPickerContent;

            static Style()
            {
                folderTexture = EditorGUIUtility.FindTexture("Folder Icon");

                folderLabelStyle = new GUIStyle(EditorStyles.foldout)
                {
                    padding = new RectOffset(17, 2, 0, 0),                                                         
                };

                usualIconPickerStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                smallIconPickerStyle = new GUIStyle(EditorStyles.miniButtonRight);

                usualIconPickerContent = new GUIContent("Select");
                smallIconPickerContent = EditorGUIUtility.IconContent("winbtn_graph");
                usualIconPickerContent.tooltip = "Select big icon";
                smallIconPickerContent.tooltip = "Select small icon";
            }
        }
    }
}