#if UNITY_2020_1_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class SerializedDictionaryDrawer : ToolboxTargetTypeDrawer
    {
        static SerializedDictionaryDrawer()
        {
            storage = new PropertyDataStorage<ReorderableListBase, CreationArgs>(false, (p, a) =>
            {
                var pairsProperty = a.pairsProperty;
                var errorProperty = a.errorProperty;

                var list = new ToolboxEditorList(pairsProperty, "Pair", true, true, false)
                {
                    Foldable = true
                };
                list.drawHeaderCallback += (rect) =>
                {
                    //cache preprocessed label to get prefab related functions
                    var label = EditorGUI.BeginProperty(rect, null, p);

                    //create additional warning message if there is a collision
                    if (errorProperty.boolValue)
                    {
                        label.text += string.Format(" [{0}]", Style.warningMessage);
                        var warningRect = rect;
                        warningRect.xMin = warningRect.xMax - Style.warningIconOffset;
                        var warningIcon = new GUIContent(EditorGuiUtility.GetHelpIcon(MessageType.Warning));
                        EditorGUI.LabelField(warningRect, warningIcon);
                    }

                    list.DrawStandardFoldout(rect, label);
                    EditorGUI.EndProperty();
                };
                list.drawFooterCallback += (rect) =>
                {
                    list.DrawStandardFooter(rect);
                };
                list.drawElementCallback += (rect, index, isActive, isFocused) =>
                {
                    var element = pairsProperty.GetArrayElementAtIndex(index);
                    var kProperty = element.FindPropertyRelative("key");
                    var vProperty = element.FindPropertyRelative("value");

                    var content = list.GetElementContent(element, index);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (kProperty != null)
                        {
                            var kOption = GUILayout.Width(Style.kGroupWidth);
                            DrawDictionaryProperty(kProperty, Style.kLabel, Style.kLabelWidth, kOption);
                        }

                        if (vProperty != null)
                        {
                            var vLabel = vProperty.hasVisibleChildren
                                ? Style.vLabel
                                : GUIContent.none;
                            DrawDictionaryProperty(vProperty, vLabel, Style.vLabelWidth);
                        }
                    }
                };
                return list;
            });
        }

        private static readonly PropertyDataStorage<ReorderableListBase, CreationArgs> storage;

        private static void DrawDictionaryProperty(SerializedProperty property, GUIContent label, float labelWidth, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.VerticalScope(Style.propertyGroupStyle, options))
            {
                var indent = property.hasVisibleChildren ? 1 : 0;
                using (new ChangeIndentScope(indent))
                {
                    EditorGUIUtility.labelWidth = labelWidth;
                    ToolboxEditorGui.DrawToolboxProperty(property, label);
                    EditorGUIUtility.labelWidth = -1;
                }
            }
        }

        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            var pairsProperty = property.FindPropertyRelative("pairs");
            var errorProperty = property.FindPropertyRelative("error");
            var drawerArgs = new CreationArgs()
            {
                pairsProperty = pairsProperty,
                errorProperty = errorProperty
            };

            storage.ReturnItem(property, drawerArgs).DoList();
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
            internal static readonly float kLabelWidth = 40.0f;
            internal static readonly float kGroupWidth = 200.0f;
            internal static readonly float vLabelWidth = 150.0f;

            internal static readonly float warningIconOffset = 25.0f;
            internal static readonly string warningMessage = "keys are not unique, it will break deserialization";

            internal static readonly GUIContent kLabel = new GUIContent("Key");
            internal static readonly GUIContent vLabel = new GUIContent("Value");

            internal static readonly GUIStyle propertyGroupStyle = new GUIStyle(EditorStyles.helpBox);
        }

        private struct CreationArgs
        {
            public SerializedProperty pairsProperty;
            public SerializedProperty errorProperty;
        }
    }
}
#endif