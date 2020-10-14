using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(FolderData))]
    internal class FolderDataDrawer : PropertyDrawer
    {
        private const string selectorEventName = "ObjectSelectorUpdated";

        private const int largeIconPickedId = 1001;
        private const int smallIconPickedId = 1002;


        private void DrawFolderByNameIcon(Rect rect)
        {
            var diff = rect.height - Style.dataByNameLabelStyle.fixedHeight;
            rect.yMin += diff;
            rect.yMax -= diff;
            GUI.Label(rect, "Aa", Style.dataByNameLabelStyle);
        }

        private void DrawFolderByPathIcon(Rect rect)
        {
            GUI.DrawTexture(rect, Style.foldersIcon);
        }

        private void DrawLabelWarningIcon(Rect rect)
        {
            GUI.DrawTexture(rect, Style.foldersIcon);
            GUI.DrawTexture(rect, Style.warningIcon);
        }

        private int GetSelectorHash(SerializedProperty property)
        {
            return property.propertyPath.GetHashCode() + property.serializedObject.GetHashCode();
        }


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
                height += Style.largeFolderHeight;
                return height;
            }

            return Style.height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("type");
            var nameProperty = property.FindPropertyRelative("name");
            var pathProperty = property.FindPropertyRelative("path");
            var toolProperty = property.FindPropertyRelative("tooltip");

            var largeIconProperty = property.FindPropertyRelative("largeIcon");
            var smallIconProperty = property.FindPropertyRelative("smallIcon");

            var isPathBased = typeProperty.intValue == 0;
            var propertyName = isPathBased
                ? pathProperty.stringValue
                : nameProperty.stringValue;

            //begin the main property
            label = EditorGUI.BeginProperty(position, label, property);
            label.text = string.IsNullOrEmpty(propertyName) ? label.text : propertyName;

            var propertyPosition = new Rect(position.x, position.y, position.width, Style.height);

            if (!(property.isExpanded = EditorGUI.Foldout(propertyPosition, property.isExpanded, label, true, Style.propertyFoldoutStyle)))
            {
                EditorGUI.EndProperty();

                //adjust position for small icon and draw it in label field
                var typeIconRect = new Rect(position.xMax - Style.smallFolderWidth,
#if UNITY_2019_3_OR_NEWER
                                            position.yMin, 
                                            Style.smallFolderWidth, 
                                            Style.smallFolderHeight);
#else
                                            position.yMin - Style.spacing,
                                            Style.smallFolderWidth,
                                            Style.smallFolderHeight);
#endif
                if (isPathBased)
                {
                    //NOTE: if path property is bigger than typical row height it means that associated value is invalid
                    var isValid = EditorGUI.GetPropertyHeight(pathProperty) <= Style.height;
                    if (isValid)
                    {
                        DrawFolderByPathIcon(typeIconRect);
                    }
                    else
                    {
                        DrawLabelWarningIcon(typeIconRect);
                    }
                }
                else
                {
                    DrawFolderByNameIcon(typeIconRect);
                }

                return;
            }

            EditorGUI.indentLevel++;

            var rawPropertyHeight = propertyPosition.height + Style.spacing;
            var summaryFieldHeight = rawPropertyHeight;

            propertyPosition.y += rawPropertyHeight;
            summaryFieldHeight += rawPropertyHeight;
            //draw the folder data type property
            EditorGUI.PropertyField(propertyPosition, typeProperty, false);
            propertyPosition.y += rawPropertyHeight;
            summaryFieldHeight += rawPropertyHeight;

            //decide what property should be drawn depending on the folder data type
            if (isPathBased)
            {
                propertyPosition.height = EditorGUI.GetPropertyHeight(pathProperty);
                EditorGUI.PropertyField(propertyPosition, pathProperty, false);
                propertyPosition.y += propertyPosition.height + Style.spacing;
                summaryFieldHeight += propertyPosition.height + Style.spacing;
                propertyPosition.height = Style.height;
            }
            else
            {
                EditorGUI.PropertyField(propertyPosition, nameProperty, false);
                propertyPosition.y += rawPropertyHeight;
                summaryFieldHeight += rawPropertyHeight;
            }

            //draw the tooltip property
            EditorGUI.PropertyField(propertyPosition, toolProperty, false);
            propertyPosition.y += rawPropertyHeight;
            summaryFieldHeight += rawPropertyHeight;

            //adjust rect for folder icons + button strip
            propertyPosition.y += Style.spacing;
            propertyPosition.x += Style.padding;
            propertyPosition.width = Style.largeFolderWidth;

            var selectorHash = GetSelectorHash(property);

            //draw normal icon property picker
            if (GUI.Button(propertyPosition, Style.largeIconPickerContent, Style.largeIconPickerStyle))
            {
                EditorGUIUtility.ShowObjectPicker<Texture>(largeIconProperty.objectReferenceValue, false, null, selectorHash + largeIconPickedId);
            }

            propertyPosition.xMin = propertyPosition.xMax;
            propertyPosition.xMax = propertyPosition.xMin + Style.smallFolderWidth;

            //draw small icon property picker
            if (GUI.Button(propertyPosition, Style.smallIconPickerContent, Style.smallIconPickerStyle))
            {
                EditorGUIUtility.ShowObjectPicker<Texture>(smallIconProperty.objectReferenceValue, false, null, selectorHash + smallIconPickedId);
            }

            //catch object selection event and assign it to proper property
            if (Event.current.commandName == selectorEventName)
            {
                //get proper action id by removing unique property hash code
                var rawPickId = EditorGUIUtility.GetObjectPickerControlID();
                var controlId = rawPickId - selectorHash;

                //determine the target property using predefined values
                SerializedProperty iconProperty = null;
                switch (controlId)
                {
                    case largeIconPickedId:
                        iconProperty = largeIconProperty;
                        break;
                    case smallIconPickedId:
                        iconProperty = smallIconProperty;
                        break;
                }

                //update the previously picked property
                if (iconProperty != null)
                {
                    var o = EditorGUIUtility.GetObjectPickerObject();
                    iconProperty.serializedObject.Update();
                    iconProperty.objectReferenceValue = o;
                    iconProperty.serializedObject.ApplyModifiedProperties();
                    //force GUI to changed state
                    GUI.changed = true;
                }
            }

            position.x += Style.padding;

            //adjust rect for each icon
            var largeFolderIconRect = new Rect(position.x,
                                               position.yMin + summaryFieldHeight,
                                               Style.largeFolderWidth,
                                               Style.largeFolderHeight);
            var smallFolderIconRect = new Rect(position.x + Style.largeFolderWidth,
                                               //adjust small rect to the large one
                                               largeFolderIconRect.y + Style.smallFolderHeight / 2 - Style.spacing,
                                               Style.smallFolderWidth,
                                               Style.smallFolderHeight);

            //draw default folder icons
            GUI.DrawTexture(largeFolderIconRect, Style.foldersIcon, ScaleMode.ScaleToFit);
            GUI.DrawTexture(smallFolderIconRect, Style.foldersIcon, ScaleMode.ScaleToFit);

            if (largeIconProperty.objectReferenceValue)
            {
                var previewTexture = largeIconProperty.objectReferenceValue as Texture;
                GUI.DrawTexture(ToolboxEditorProject.GetLargeIconRect(largeFolderIconRect), previewTexture, ScaleMode.ScaleToFit);
            }

            if (smallIconProperty.objectReferenceValue)
            {
                var previewTexture = smallIconProperty.objectReferenceValue as Texture;
                GUI.DrawTexture(ToolboxEditorProject.GetSmallIconRect(smallFolderIconRect), previewTexture, ScaleMode.ScaleToFit);
            }

            //end property and fix indent level
            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 12.0f;

            internal const float largeFolderWidth = 64.0f;
            internal const float largeFolderHeight = 64.0f;

            internal const float smallFolderWidth = 16.0f;
            internal const float smallFolderHeight = 16.0f;

            internal static readonly GUIStyle dataByNameLabelStyle;
            internal static readonly GUIStyle propertyFoldoutStyle;
            internal static readonly GUIStyle largeIconPickerStyle;
            internal static readonly GUIStyle smallIconPickerStyle;

            internal static readonly GUIContent largeIconPickerContent;
            internal static readonly GUIContent smallIconPickerContent;

            internal static readonly Texture2D foldersIcon;
            internal static readonly Texture2D warningIcon;

            static Style()
            {
#if UNITY_2019_3_OR_NEWER
                dataByNameLabelStyle = new GUIStyle(EditorStyles.miniLabel);
#else
                dataByNameLabelStyle = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleCenter
                };
#endif
                propertyFoldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    padding = new RectOffset(17, 2, 0, 0),
                };

                largeIconPickerStyle = new GUIStyle(EditorStyles.miniButtonLeft)
                {
                    fixedHeight = 16.0f,
                    fontSize = 9
                };
                smallIconPickerStyle = new GUIStyle(EditorStyles.miniButtonRight)
                {
                    fixedHeight = 16.0f,
                    padding = new RectOffset(2, 2, 2, 2)
                };

                largeIconPickerContent = new GUIContent("Select");
                smallIconPickerContent = EditorGUIUtility.IconContent("RawImage Icon");
                largeIconPickerContent.tooltip = "Select large icon";
                smallIconPickerContent.tooltip = "Select small icon";

                foldersIcon = EditorGUIUtility.FindTexture("Folder Icon");
                warningIcon = EditorGUIUtility.FindTexture("console.warnicon.sml");
            }
        }
    }
}