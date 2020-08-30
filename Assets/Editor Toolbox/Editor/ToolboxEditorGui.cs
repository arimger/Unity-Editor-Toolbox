using System;

using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Contains useful and ready-to-use Editor controls.
    /// </summary>
    public static partial class ToolboxEditorGui
    {
        /// <summary>
        /// Draws horizontal line. Uses built-in layouting system.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawLayoutLine(float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLayoutLine(thickness, padding, Style.standardLineColor);
        }

        /// <summary>
        /// Draws horizontal line. Uses built-in layouting system.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawLayoutLine(float thickness, float padding, Color color)
        {
            DrawLine(EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness)), thickness, padding, color);
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        public static void DrawLine(Rect rect, float thickness = 0.75f, float padding = 6.0f)
        {
            DrawLine(rect, thickness, padding, Style.standardLineColor);
        }

        /// <summary>
        /// Draws horizontal line.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="thickness"></param>
        /// <param name="padding"></param>
        /// <param name="color"></param>
        public static void DrawLine(Rect rect, float thickness, float padding, Color color)
        {
            rect.y += padding / 2;
            rect.height = thickness;

            EditorGUI.DrawRect(rect, color);
        }

        /// <summary>
        /// Creates asset preview for given <see cref="Object"/>. Uses built-in layouting system.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name=""></param>
        public static void DrawLayoutAssetPreview(Object asset, float width = 64, float height = 64)
        {
            var previewTexture = AssetPreview.GetAssetPreview(asset);

            if (!previewTexture)
            {
                return;
            }

            var style = new GUIStyle();
            style.normal.background = previewTexture;
            width = Mathf.Clamp(width, 0, previewTexture.width);
            height = Mathf.Clamp(height, 0, previewTexture.height);
            var previewOptions = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(width),
                GUILayout.MaxHeight(height)
            };

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(Style.boxedSkinStyle);
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(true, height, previewOptions), GUIContent.none, style);
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws header-like label in form of foldout. Uses built-in layouting system.
        /// </summary>
        /// <returns></returns>
        public static bool DrawLayoutHeaderFoldout(bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle foldoutStyle)
        {
            const float headerHeight = 25.0f;

            var rect = GUILayoutUtility.GetRect(1, headerHeight);
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            return DrawHeaderFoldout(rect, foldout, label, toggleOnLabelClick, foldoutStyle);
        }

        /// <summary>
        /// Draws header-like label in form of foldout.
        /// </summary>
        /// <returns></returns>
        public static bool DrawHeaderFoldout(Rect rect, bool foldout, GUIContent label, bool toggleOnLabelClick, GUIStyle foldoutStyle)
        {
            const float xPadding = 15.0f;
#if UNITY_2019_3_OR_NEWER
            const float yPadding = 7.0f;
#else
            const float yPadding = 2.0f;
#endif

            if (Event.current.type == EventType.Repaint)
            {
                Style.boxedSkinStyle.Draw(rect, false, false, false, false);
            }

            rect.xMin += xPadding;
            rect.yMin -= yPadding - (rect.height - foldoutStyle.fontSize) / 2;

            return EditorGUI.Foldout(rect, foldout, label, toggleOnLabelClick, foldoutStyle);
        }

        /// <summary>
        /// Creates a tooltip-only label field.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="tooltip"></param>
        public static void DrawTooltip(Rect rect, string tooltip)
        {
            if (tooltip == null || tooltip.Length == 0)
            {
                return;
            }

            GUI.Label(rect, new GUIContent(string.Empty, tooltip));
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="texture"></param>
        public static void DrawTexture(Rect rect, Texture texture)
        {
            DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="texture"></param>
        public static void DrawTexture(Rect rect, Texture texture, ScaleMode scaleMode)
        {
            DrawTexture(rect, texture, scaleMode, true);
        }

        /// <summary>
        /// Draws texture using the built-in <see cref="GUI"/> class.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="texture"></param>
        public static void DrawTexture(Rect rect, Texture texture, ScaleMode scaleMode, bool alphaBlend)
        {
            GUI.DrawTexture(rect, texture, scaleMode, alphaBlend);
        }


        internal static class Style
        {
            internal static readonly Color standardLineColor = new Color(0.3f, 0.3f, 0.3f);

            internal static readonly GUIStyle boxedSkinStyle;
            internal static readonly GUIStyle labelSkinStyle;
            internal static readonly GUIStyle buttonSkinStyle;
            internal static readonly GUIStyle miniButtonStyle;

            internal static readonly GUIContent warningContent;

            static Style()
            {
                boxedSkinStyle = new GUIStyle(GUI.skin.box);
                labelSkinStyle = new GUIStyle(GUI.skin.label);
                buttonSkinStyle = new GUIStyle(GUI.skin.button);
                miniButtonStyle = new GUIStyle(EditorStyles.miniButton);

                warningContent = EditorGUIUtility.IconContent("console.warnicon.sml");
            }
        }
    }

    public static partial class ToolboxEditorGui
    {
        /// <summary>
        /// Draws <see cref="ReorderableList"/> as drawer list instace used within the <see cref="ToolboxEditorSettingsEditor"/> class.
        /// </summary>
        /// <param name="drawersList"></param>
        /// <param name="drawersTitle"></param>
        /// <param name="assignButtonLabel"></param>
        /// <param name="foldoutStyle"></param>
        /// <returns></returns>
        internal static bool DrawDrawerList(ReorderableList drawersList, string drawersTitle, string assignButtonLabel, GUIStyle foldoutStyle)
        {
            GUILayout.BeginHorizontal();
            var expanded = DrawListFoldout(drawersList, foldoutStyle, drawersTitle);
            GUILayout.FlexibleSpace();
            var pressed = GUILayout.Button(assignButtonLabel, Style.miniButtonStyle);
            GUILayout.EndHorizontal();

            if (expanded)
            {
                drawersList.DoLayoutList();
            }

            return pressed;
        }

        /// <summary>
        /// Draws <see cref="ReorderableList"/> with an additional foldout.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        internal static bool DrawListFoldout(ReorderableList list, GUIStyle style)
        {
            return DrawListFoldout(list, style, list.List.displayName);
        }

        /// <summary>
        /// Draws <see cref="ReorderableList"/> with an additional foldout and a custom label.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        internal static bool DrawListFoldout(ReorderableList list, GUIStyle style, string label)
        {
            list.HasHeader = false;
            return list.List.isExpanded = EditorGUILayout.Foldout(list.List.isExpanded, label, true, style);
        }


        /// <summary>
        /// Creates <see cref="ReorderableList"/> using standard background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateRoundList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize);
        }

        /// <summary>
        /// Creates <see cref="ReorderableList"/> using a non-standard(box style) background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateBoxedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize)
            {
                drawHeaderBackgroundCallback = (Rect rect) =>
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxedSkinStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawMiddleBackgroundCallback = (Rect rect) =>
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxedSkinStyle.Draw(rect, false, false, false, false);
                    }
                },
                drawFooterBackgroundCallback = (Rect rect) =>
                {
                    rect.y -= EditorGUIUtility.standardVerticalSpacing / 2;
                    if (Event.current.type == EventType.Repaint)
                    {
                        Style.boxedSkinStyle.Draw(rect, false, false, false, false);
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
        /// Creates <see cref="ReorderableList"/> using a non-standard(lined style) background.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static ReorderableList CreateLinedList(SerializedProperty property, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            return new ReorderableList(property, elementLabel, draggable, true, fixedSize)
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
        /// Creates <see cref="ReorderableList"/> using provided <see cref="ListStyle"/> type.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ReorderableList CreateList(SerializedProperty property, ListStyle style, string elementLabel = null, bool fixedSize = false, bool draggable = true)
        {
            switch (style)
            {
                case ListStyle.Boxed: return CreateBoxedList(property, elementLabel, fixedSize, draggable);
                case ListStyle.Lined: return CreateLinedList(property, elementLabel, fixedSize, draggable);
                case ListStyle.Round: return CreateRoundList(property, elementLabel, fixedSize, draggable);
                default: return null;
            }
        }
    }

    public static partial class ToolboxEditorGui
    {
        [Obsolete]
        public static void DrawToolboxProperty(Rect position, SerializedProperty property)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public static void DrawToolboxProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Draws property using additional <see cref="PropertyDrawer"/>s and <see cref="Drawers.ToolboxAttributeDrawer"/>s.
        /// Uses built-in layouting system.
        /// </summary>
        /// <param name="property"></param>
        public static void DrawLayoutToolboxProperty(SerializedProperty property)
        {
            ToolboxDrawerModule.GetPropertyHandler(property).OnGuiLayout();
        }

        /// <summary>
        /// Draws property in default way but children will support Toolbox drawers.
        /// Uses built-in layouting system.
        /// </summary>
        /// <param name="property"></param>
        public static void DrawLayoutDefaultProperty(SerializedProperty property)
        {
            DrawLayoutDefaultProperty(property, null);
        }

        /// <summary>
        /// Draws property in default way but children will support Toolbox drawers.
        /// Uses built-in layouting system.
        /// </summary>
        /// <param name="property"></param>
        public static void DrawLayoutDefaultProperty(SerializedProperty property, GUIContent label)
        {
            //draw standard foldout with built-in operations(like prefabs handling)
            //to re-create native steps:
            // - get foldout rect
            // - begin property using EditorGUI.BeginProperty method
            // - read current drag events
            // - draw foldout
            // - close property using EditorGUI.EndProperty method
            if (!EditorGUILayout.PropertyField(property, label, false))
            {
                return;
            }

            var currentEvent = Event.current;
            //use current event if drag was performed otherwise, it will cause an error
            if (currentEvent.type == EventType.DragPerform && GUI.changed) currentEvent.Use();

            var iterateThroughChildren = true;

            //handle property references
            var iterProperty = property.Copy();
            var lastProperty = iterProperty.GetEndProperty();

            EditorGUI.indentLevel++;

            //iterate over all children(but only one level depth)
            while (iterProperty.NextVisible(iterateThroughChildren))
            {
                if (SerializedProperty.EqualContents(iterProperty, lastProperty))
                {
                    break;
                }

                iterateThroughChildren = false;

                //handle current property using Toolbox drawers
                DrawLayoutToolboxProperty(iterProperty.Copy());
            }

            //restore old indent level
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws property in native-default way. Uses built-in layouting system.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public static void DrawLayoutNativeProperty(SerializedProperty property)
        {
            DrawLayoutNativeProperty(property, null);
        }

        /// <summary>
        /// Draws property in native-default way. Uses built-in layouting system.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public static void DrawLayoutNativeProperty(SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
        }

        /// <summary>
        /// Draws provided property as a warning label. Uses built-in layouting system.
        /// </summary>
        public static void DrawLayoutEmptyProperty(SerializedProperty property)
        {
            DrawLayoutEmptyProperty(property, null);
        }

        /// <summary>
        /// Draws provided property as a warning label. Uses built-in layouting system.
        /// </summary>
        public static void DrawLayoutEmptyProperty(SerializedProperty property, GUIContent label)
        {
            DrawEmptyProperty(GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight), property, label);
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
            var xMin = position.xMin;
            var xMax = position.xMax;
            var yMin = position.yMin;
            var yMax = position.yMax;

            var dim = position.height + EditorGUIUtility.standardVerticalSpacing * 2;
            position.width = dim;
            position.height = dim;

            EditorGUI.LabelField(position, Style.warningContent);

            position.xMin = xMin + position.width;
            position.xMax = xMax;
            position.yMin = yMin;
            position.yMax = yMax;

            var content = label ?? new GUIContent(property.displayName);

            EditorGUI.LabelField(position, content);
        }
    }
}