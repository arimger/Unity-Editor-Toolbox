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
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            DrawLine(rect, thickness, padding, color);
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        public static void DrawLine(Rect rect, float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLine(rect, thickness, padding, new Color(0.3f, 0.3f, 0.3f));
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        public static void DrawLine(Rect rect, float thickness, float padding, Color color)
        {
            rect = new Rect(rect.x, rect.y + padding / 2, rect.width, thickness);
            EditorGUI.DrawRect(rect, color);
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
            return EditorGUI.Foldout(rect, foldout, GUIContent.none, toggleOnLabelClick,  foldoutStyle);
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
    }

    public static partial class ToolboxEditorGui
    {
        /// <summary>
        /// Creates <see cref="ReorderableList"/> using standard background.
        /// </summary>
        public static ReorderableList CreateRoundList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true)
        {
            return new ReorderableList(property, elementLabel, draggable, hasHeader, fixedSize);
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using a non-standard (boxed style) background.
        /// </summary>
        public static ReorderableList CreateBoxedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true)
        {
            var backgroundStyle = new GUIStyle("box");
            return new ReorderableList(property, elementLabel, draggable, hasHeader, fixedSize)
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
                FooterHeight = 17
#else
                FooterHeight = 14
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using a non-standard (lined style) background.
        /// </summary>
        public static ReorderableList CreateLinedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true)
        {
            return new ReorderableList(property, elementLabel, draggable, hasHeader, fixedSize)
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
                HeaderHeight = 21,
                FooterHeight = 17
#else
                FooterHeight = 15
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> without any additional background.
        /// </summary>
        public static ReorderableList CreateClearList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true)
        {
            return new ReorderableList(property, elementLabel, draggable, hasHeader, fixedSize)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                { },
                drawMiddleBackgroundCallback = (Rect rect) =>
                { },
                drawFooterBackgroundCallback = (Rect rect) =>
                { },
#if UNITY_2019_3_OR_NEWER
                FooterHeight = 17
#else
                FooterHeight = 15
#endif
            };
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using provided <see cref="ListStyle"/> type.
        /// </summary>
        public static ReorderableList CreateList(SerializedProperty property, ListStyle style, string elementLabel = null, bool fixedSize = false, bool draggable = true, bool hasHeader = true)
        {
            switch (style)
            {
                case ListStyle.Boxed: return CreateBoxedList(property, elementLabel, fixedSize, draggable, hasHeader);
                case ListStyle.Lined: return CreateLinedList(property, elementLabel, fixedSize, draggable, hasHeader);
                case ListStyle.Round: return CreateRoundList(property, elementLabel, fixedSize, draggable, hasHeader);
                default: return null;
            }
        }
    }

    public static partial class ToolboxEditorGui
    {
        [Obsolete]
        public static void DrawToolboxProperty(Rect position, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
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
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawDefaultProperty(SerializedProperty property)
        {
            DrawDefaultProperty(property, null);
        }

        /// <summary>
        /// Draws property in default way but children will support Toolbox drawers.
        /// Uses built-in layouting system.
        /// </summary>
        public static void DrawDefaultProperty(SerializedProperty property, GUIContent label)
        {
            //draw standard foldout with built-in operations (like prefabs handling)
            //native steps to re-create:
            // - get foldout rect
            // - begin property using EditorGUI.BeginProperty method
            // - read current drag events
            // - draw foldout
            // - close property using EditorGUI.EndProperty method
            if (!EditorGUILayout.PropertyField(property, label, false))
            {
                return;
            }

            var enterChildren = true;
            //cache all needed property references
            var targetProperty = property.Copy();
            var endingProperty = property.GetEndProperty();

            EditorGUI.indentLevel++;
            //iterate over all children (but only 1 level depth)
            while (targetProperty.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(targetProperty, endingProperty))
                {
                    break;
                }

                enterChildren = false;
                //handle current property using Toolbox features
                DrawToolboxProperty(targetProperty.Copy());
            }

            EditorGUI.indentLevel--;
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
            EditorGUI.LabelField(position, label ?? new GUIContent(property.displayName));
        }
    }
}