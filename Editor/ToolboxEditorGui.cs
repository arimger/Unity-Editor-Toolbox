using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Contains useful and ready-to-use Editor controls.
    /// </summary>
    public static partial class ToolboxEditorGui
    {
        private static Rect GetLineRect(Rect rect, float thickness, float padding, bool horizontal)
        {
            return horizontal
                ? new Rect(rect.x, rect.y + padding / 2, rect.width, thickness)
                : new Rect(rect.x + padding / 2, rect.y, thickness, rect.height);
        }


        /// <summary>
        /// Draws horizontal line.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawLine(float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLine(thickness, padding, new Color(0.3f, 0.3f, 0.3f));
        }

        /// <summary>
        /// Draws horizontal line.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawLine(float thickness, float padding, Color color)
        {
            DrawLine(thickness, padding, color, true);
        }

        /// <summary>
        /// Draws horizontal line.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawLine(float thickness, float padding, Color color, bool horizontal)
        {
            DrawLine(thickness, padding, color, horizontal, false);
        }

        /// <summary>
        /// Draws horizontal line.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawLine(float thickness, float padding, Color color, bool horizontal, bool applyIndent)
        {
            var area = padding + thickness;
            var rect = horizontal
                ? EditorGUILayout.GetControlRect(GUILayout.Height(area), GUILayout.ExpandWidth(true))
                : EditorGUILayout.GetControlRect(GUILayout.Width(area), GUILayout.ExpandHeight(true));
            if (applyIndent)
            {
                rect = EditorGUI.IndentedRect(rect);
            }

            DrawLine(rect, thickness, padding, color, horizontal);
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        public static void DrawLine(Rect rect, float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLine(rect, thickness, padding, new Color(0.3f, 0.3f, 0.3f), true);
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        public static void DrawLine(Rect rect, float thickness, float padding, Color color, bool horizontal)
        {
            EditorGUI.DrawRect(GetLineRect(rect, thickness, padding, horizontal), color);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// Uses built-in layouting system.
        /// </summary>
        public static bool DrawHeaderFoldout(bool foldout, GUIContent label, bool toggleOnLabelClick)
        {
            return DrawHeaderFoldout(foldout, label, toggleOnLabelClick, null);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// Uses built-in layouting system.
        /// </summary>
        public static bool DrawHeaderFoldout(bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle)
        {
            return DrawHeaderFoldout(foldout, label, toggleOnLabelClick, headerStyle, new GUIStyle());
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// Uses built-in layouting system.
        /// </summary>
        public static bool DrawHeaderFoldout(bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle, GUIStyle backgroundStyle)
        {
            return DrawHeaderFoldout(foldout, label, toggleOnLabelClick, headerStyle, backgroundStyle, EditorStyles.inspectorFullWidthMargins);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// Uses built-in layouting system.
        /// </summary>
        public static bool DrawHeaderFoldout(bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle, GUIStyle backgroundStyle, GUIStyle foldoutStyle)
        {
            //where 30.0f - header height, 2.0f - additional padding
            var rect = GUILayoutUtility.GetRect(1, 30.0f);
            rect.xMin = EditorGUIUtility.standardVerticalSpacing;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            return DrawHeaderFoldout(rect, foldout, label, toggleOnLabelClick, headerStyle, backgroundStyle, foldoutStyle);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// </summary>
        public static bool DrawHeaderFoldout(Rect rect, bool foldout, GUIContent label, bool toggleOnLabelClick)
        {
            return DrawHeaderFoldout(rect, foldout, label, toggleOnLabelClick, null);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// </summary>
        public static bool DrawHeaderFoldout(Rect rect, bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle)
        {
            return DrawHeaderFoldout(rect, foldout, label, toggleOnLabelClick, headerStyle, new GUIStyle());
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// </summary>
        public static bool DrawHeaderFoldout(Rect rect, bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle, GUIStyle backgroundStyle)
        {
            return DrawHeaderFoldout(rect, foldout, label, toggleOnLabelClick, headerStyle, backgroundStyle, EditorStyles.inspectorFullWidthMargins);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// </summary>
        public static bool DrawHeaderFoldout(Rect rect, bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle headerStyle, GUIStyle backgroundStyle, GUIStyle foldoutStyle)
        {
            //draw boxed background
            if (Event.current.type == EventType.Repaint)
            {
                backgroundStyle.Draw(rect, false, false, false, false);
            }

            //create header label with additional padding
            rect.xMin += 10.0f;
            EditorGUI.LabelField(rect, label, headerStyle);
            rect.xMin -= 10.0f;

            //create final foldout without label and arrow
            return EditorGUI.Foldout(rect, foldout, GUIContent.none, toggleOnLabelClick, foldoutStyle);
        }

        /// <summary>
        /// Creates a tooltip-only label field.
        /// </summary>
        public static void DrawTooltip(Rect rect, string tooltip)
        {
            GUI.Label(rect, new GUIContent(string.Empty, tooltip));
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        public static void DrawTexture(Rect rect, Texture texture)
        {
            DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        public static void DrawTexture(Rect rect, Texture texture, ScaleMode scaleMode)
        {
            DrawTexture(rect, texture, scaleMode, true);
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        public static void DrawTexture(Rect rect, Texture texture, ScaleMode scaleMode, bool alphaBlend)
        {
            GUI.DrawTexture(rect, texture, scaleMode, alphaBlend);
        }

        public static void DrawMinMaxSlider(Rect rect, string label, ref float xValue, ref float yValue, float minValue, float maxValue)
        {
            DrawMinMaxSlider(rect, new GUIContent(label), ref xValue, ref yValue, minValue, maxValue);
        }

        public static void DrawMinMaxSlider(Rect rect, GUIContent label, ref float xValue, ref float yValue, float minValue, float maxValue)
        {
            var padding = 8.0f;
            var labelWidth = EditorGUIUtility.labelWidth;
            var fieldWidth = EditorGUIUtility.fieldWidth;

            var minFieldRect = new Rect(rect.xMin + labelWidth, rect.y, fieldWidth, rect.height);
            var maxFieldRect = new Rect(rect.xMax - fieldWidth, rect.y, fieldWidth, rect.height);
            var labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            //set slider rect between min and max fields + additional padding
            var sliderRect = new Rect(rect.x + labelWidth + fieldWidth + padding,
                                      rect.y,
                                      rect.width - labelWidth - fieldWidth * 2 - padding * 2,
                                      rect.height);

            //begin drawing using GUI methods
            EditorGUI.LabelField(labelRect, label);
            EditorGUI.BeginChangeCheck();
            xValue = EditorGUI.FloatField(minFieldRect, xValue);
            yValue = EditorGUI.FloatField(maxFieldRect, yValue);
            EditorGUI.MinMaxSlider(sliderRect, ref xValue, ref yValue, minValue, maxValue);

            //values validation (xValue can't be higher than yValue etc.)
            xValue = Mathf.Clamp(xValue, minValue, Mathf.Min(maxValue, yValue));
            yValue = Mathf.Clamp(yValue, Mathf.Max(minValue, xValue), maxValue);
        }

        public static void BoldLabel(Rect rect, string label)
        {
            BoldLabel(rect, new GUIContent(label));
        }

        public static void BoldLabel(Rect rect, GUIContent label)
        {
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
        }
    }

    public static partial class ToolboxEditorGui
    {
        private static EditorWindow lastTargetedWindow;


        /// <summary>
        /// Checks if user is still focusing the proper (searchable) window.
        /// </summary>
        private static bool IsNonTargetWindowFocused()
        {
            return lastTargetedWindow && lastTargetedWindow != EditorWindow.mouseOverWindow;
        }

        private static void OnPopupWindowCreated()
        {
            lastTargetedWindow = EditorWindow.focusedWindow;
        }

        private static void OnPopupWindowUpdated()
        {
            //handle situation when inspector window captures ScrollWheel event
            if (IsNonTargetWindowFocused())
            {
                //NOTE: unfortunately PopupWidnows are not indpendent and we have to
                //override internal events to prevent scrolling the Inspector Window
                if (Event.current.type == EventType.ScrollWheel)
                {
                    Event.current.Use();
                }
            }
        }


        public static void DrawSearchablePopup(Rect rect, GUIContent label, int currentIndex, string[] options, Action<int> onSelect)
        {
            DrawSearchablePopup(rect, label, currentIndex, options, onSelect, EditorStyles.miniPullDown);
        }

        public static void DrawSearchablePopup(Rect rect, GUIContent label, int currentIndex, string[] options, Action<int> onSelect, GUIStyle style)
        {
            if (EditorGUI.DropdownButton(rect, label, FocusType.Keyboard, style))
            {
                try
                {
                    SearchablePopup.Show(rect, currentIndex, options, onSelect);
                }
                catch (ExitGUIException)
                {
                    OnPopupWindowCreated();
                    throw;
                }
            }

            OnPopupWindowUpdated();
        }
    }

    public static partial class ToolboxEditorGui
    {
        /// <summary>
        /// Creates <see cref="ReorderableList"/> using standard background.
        /// </summary>
        public static ReorderableListBase CreateRoundList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true, bool hasLabels = true, bool foldable = false)
        {
            return new ToolboxEditorList(property, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable);
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using a non-standard (boxed style) background.
        /// </summary>
        public static ReorderableListBase CreateBoxedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true, bool hasLabels = true, bool foldable = false)
        {
            var backgroundStyle = new GUIStyle("box");
            return new ToolboxEditorList(property, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        backgroundStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawMiddleBackgroundCallback = (Rect rect) =>
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        backgroundStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawFooterBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        backgroundStyle.Draw(rect, false, false, false, false);
                    }
                },
#if UNITY_2019_3_OR_NEWER
                FooterHeight = 17.0f
#else
                FooterHeight = 14.0f
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using a non-standard (lined style) background.
        /// </summary>
        public static ReorderableListBase CreateLinedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true, bool hasLabels = true, bool foldable = false)
        {
            return new ToolboxEditorList(property, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing;
                    DrawLine(rect);
                },
                drawMiddleBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing * 2;
                    DrawLine(rect);
                },
                drawFooterBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing * 2 - rect.height;
                    DrawLine(rect);
                },
#if UNITY_2019_3_OR_NEWER
                HeaderHeight = 21.0f,
                FooterHeight = 17.0f
#else
                FooterHeight = 15.0f
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> without any additional background.
        /// </summary>
        public static ReorderableListBase CreateClearList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true, bool hasLabels = true, bool foldable = false)
        {
            return new ToolboxEditorList(property, elementLabel, draggable, hasHeader, fixedSize, hasLabels, foldable)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                { },
                drawMiddleBackgroundCallback = (Rect rect) =>
                { },
                drawFooterBackgroundCallback = (Rect rect) =>
                { },
#if UNITY_2019_3_OR_NEWER
                FooterHeight = 17.0f
#else
                FooterHeight = 15.0f
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using provided <see cref="ListStyle"/> type.
        /// </summary>
        public static ReorderableListBase CreateList(SerializedProperty property, ListStyle style, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true, bool hasLabels = true, bool foldable = false)
        {
            switch (style)
            {
                case ListStyle.Boxed: return CreateBoxedList(property, elementLabel, fixedSize, draggable, hasHeader, hasLabels, foldable);
                case ListStyle.Lined: return CreateLinedList(property, elementLabel, fixedSize, draggable, hasHeader, hasLabels, foldable);
                case ListStyle.Round: return CreateRoundList(property, elementLabel, fixedSize, draggable, hasHeader, hasLabels, foldable);
                default: return null;
            }
        }
    }

    public static partial class ToolboxEditorGui
    {
        [Obsolete("Toolbox-related functions are only layout-based.")]
        public static void DrawToolboxProperty(Rect position, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Toolbox-related functions are only layout-based.")]
        public static void DrawToolboxProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws property using additional <see cref="PropertyDrawer"/>s and <see cref="Drawers.ToolboxAttributeDrawer"/>s.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawToolboxProperty(SerializedProperty property)
        {
            ToolboxDrawerModule.GetPropertyHandler(property)?.OnGuiLayout();
        }

        /// <summary>
        /// Draws property using additional <see cref="PropertyDrawer"/>s and <see cref="Drawers.ToolboxAttributeDrawer"/>s.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawToolboxProperty(SerializedProperty property, GUIContent label)
        {
            ToolboxDrawerModule.GetPropertyHandler(property)?.OnGuiLayout(label);
        }

        /// <summary>
        /// Draws property in default way but children will support Toolbox drawers.
        /// 'Default way' means it will create foldout-based property if children are visible.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawDefaultProperty(SerializedProperty property)
        {
            DrawDefaultProperty(property, null);
        }

        /// <summary>
        /// Draws property in default way but children will support Toolbox drawers.
        /// 'Default way' means it will create foldout-based property if children are visible.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawDefaultProperty(SerializedProperty property, GUIContent label)
        {
            DrawDefaultProperty(property, label, DrawNativeProperty, DrawToolboxProperty);
        }

        /// <summary>
        /// Draws property in default way but each single property is handled by custom action.
        /// 'Default way' means it will create foldout-based property if children are visible.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawDefaultProperty(SerializedProperty property, GUIContent label,
            Action<SerializedProperty, GUIContent> drawParentAction, Action<SerializedProperty> drawElementAction)
        {
            if (!PropertyUtility.HasVisibleChildrenFields(property))
            {
                drawParentAction(property, label);
                return;
            }

            //draw standard foldout with built-in operations (like prefabs handling)
            //native steps to re-create:
            // - get foldout rect
            // - begin property using EditorGUI.BeginProperty method
            // - read current drag events
            // - draw foldout
            // - close property using EditorGUI.EndProperty method
            using (var propertyScope = new PropertyScope(property, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                EditorGUI.indentLevel++;
                DrawPropertyChildren(property, drawElementAction);
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Draws property's children but only one level deep.
        /// </summary>
        public static void DrawPropertyChildren(SerializedProperty property)
        {
            DrawPropertyChildren(property, DrawToolboxProperty);
        }

        /// <summary>
        /// Draws property's children but only one level deep.
        /// </summary>
        public static void DrawPropertyChildren(SerializedProperty property, Action<SerializedProperty> drawElementAction)
        {
            var enterChildren = true;
            //cache all needed property references
            var targetProperty = property.Copy();
            var endingProperty = property.GetEndProperty();
            //iterate over all children (but only 1 level depth)
            while (targetProperty.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(targetProperty, endingProperty))
                {
                    break;
                }

                enterChildren = false;
                var childProperty = targetProperty.Copy();
                //handle current property using Toolbox features
                drawElementAction(childProperty);
            }
        }

        /// <summary>
        /// Draws property in native-default way.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawNativeProperty(SerializedProperty property)
        {
            DrawNativeProperty(property, null);
        }

        /// <summary>
        /// Draws property in native-default way.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawNativeProperty(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
        }

        /// <summary>
        /// Draws provided property as a warning label.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawEmptyProperty(SerializedProperty property)
        {
            DrawEmptyProperty(property, null);
        }

        /// <summary>
        /// Draws provided property as a warning label.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawEmptyProperty(SerializedProperty property, GUIContent label)
        {
            var rect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            DrawEmptyProperty(rect, property, label);
        }

        /// <summary>
        /// Draws provided property as a warning label.
        /// </summary>
        public static void DrawEmptyProperty(Rect position, SerializedProperty property)
        {
            DrawEmptyProperty(position, property, null);
        }

        /// <summary>
        /// Draws provided property as a warning label.
        /// </summary>
        public static void DrawEmptyProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            label = label ?? new GUIContent(property.displayName);
#if UNITY_2019_1_OR_NEWER
            label.image = EditorGuiUtility.GetHelpIcon(MessageType.Warning);
#endif
            EditorGUI.LabelField(position, label);
        }

        /// <summary>
        /// Layout-based equivalent of the <see cref="EditorGUI.BeginProperty(Rect, GUIContent, SerializedProperty)"/> method.
        /// </summary>
        public static void BeginProperty(SerializedProperty property, ref GUIContent label, out Rect position)
        {
            var rowHeight = EditorGUIUtility.singleLineHeight;
            position = EditorGUILayout.GetControlRect(true, rowHeight);
            label = EditorGUI.BeginProperty(position, label, property);
        }

        /// <summary>
        /// Layout-based equivalent of the <see cref="EditorGUI.EndProperty"/> method.
        /// </summary>
        public static void CloseProperty()
        {
            EditorGUI.EndProperty();
        }
    }
}