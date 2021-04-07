#if UNITY_2020_1_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class SerializedDictionaryDrawer : ToolboxTargetTypeDrawer
    {
        private void DrawSizeProperty(SerializedProperty property)
        {
            //create horizontal group for size + add button
            using (new EditorGUILayout.HorizontalScope())
            {
                var size = property.GetSize();
                ToolboxEditorGui.DrawNativeProperty(size);
                var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(Style.appendButtonWidth),
                                                          GUILayout.Height(Style.appendButtonWidth));
                if (GUI.Button(rect, Style.appendContent, Style.appendButtonStyle))
                {
                    AppendElement(property);
                }
            }
        }

        private void DrawElementsBody(SerializedProperty property)
        {
            //draw all available dictionary elements + remove button
            for (var i = 0; i < property.arraySize; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                ToolboxEditorGui.DrawToolboxProperty(element);
                if (element.isExpanded)
                {
                    var buttonRect = GUILayoutUtility.GetRect(0, 0, GUILayout.Height(Style.removeButtonHeight));
                    //adjust rect to the current indentation level
                    buttonRect = EditorGUI.IndentedRect(buttonRect);
                    if (GUI.Button(buttonRect, Style.removeContent, EditorStyles.miniButton))
                    {
                        RemoveElement(property, i);
                    }

                    //apply additional spacing for expanded elements
                    GUILayout.Space(Style.expandedSpacing);
                }
            }
        }

        private void AppendElement(SerializedProperty property)
        {
            property.arraySize += 1;
            var newElement = property.GetArrayElementAtIndex(property.arraySize - 1);
            newElement.isExpanded = true;
        }

        private void RemoveElement(SerializedProperty property, int index)
        {
            property.DeleteArrayElementAtIndex(index);
        }


        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            var pairsProperty = property.FindPropertyRelative("pairs");
            var errorProperty = property.FindPropertyRelative("error");

            //create a standard property scope
            using (var propertyScope = new PropertyScope(pairsProperty, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                EditorGUI.indentLevel++;
                DrawSizeProperty(pairsProperty);
                DrawElementsBody(pairsProperty);
                EditorGUI.indentLevel--;
            }

            //create additional default information about key collisions
            if (errorProperty.boolValue)
            {
                EditorGUILayout.HelpBox(Style.warningContent.text, MessageType.Warning);
            }
        }

        public override Type GetTargetType()
        {
            return typeof(SerializedDictionary<,>);
        }

        public override bool UseForChildren()
        {
            return false;
        }


        private static class Style
        {
            internal static readonly float expandedSpacing = 4.0f;

            internal static readonly float appendButtonWidth = 28.0f;
            internal static readonly float appendButtonHeight = 20.0f;
            internal static readonly float removeButtonWidth = 0.0f;
            internal static readonly float removeButtonHeight = 14.0f;

            internal static readonly GUIStyle appendButtonStyle;
            internal static readonly GUIStyle removeButtonStyle;

            internal static readonly GUIContent appendContent;
            internal static readonly GUIContent removeContent;

            internal static readonly GUIContent warningContent = new GUIContent("Some keys are not unique, it will break deserialization.");

            static Style()
            {
                appendButtonStyle = new GUIStyle("miniButton")
                {
                    fixedHeight = appendButtonHeight
                };
                removeButtonStyle = new GUIStyle("miniButton");

                appendContent = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to dictionary");
                removeContent = EditorGUIUtility.TrTextContent("Remove", "Remove pair from dictionary");
            }
        }
    }
}
#endif