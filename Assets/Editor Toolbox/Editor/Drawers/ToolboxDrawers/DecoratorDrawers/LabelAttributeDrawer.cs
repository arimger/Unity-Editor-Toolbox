using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LabelAttributeDrawer : ToolboxDecoratorDrawer<LabelAttribute>
    {
        protected override void OnGuiBeginSafe(LabelAttribute attribute)
        {
            //determine proper styles for scope and text
            var scopeStyle = GetScopeStyle(attribute.SkinStyle);
            var labelStyle = GetLabelStyle(attribute.FontStyle);

            labelStyle.alignment = attribute.Alignment;
            labelStyle.fontStyle = attribute.FontStyle;

            //create (optionally) the vertical scope group
            using (CreateScopeIfNeeded(scopeStyle))
            {
                EditorGUILayout.LabelField(GetContent(attribute), labelStyle);
            }
        }


        private static GUIStyle GetLabelStyle(FontStyle style)
        {
            return Style.labelStyle;
        }

        private static GUIStyle GetScopeStyle(SkinStyle style)
        {
            switch (style)
            {
                case SkinStyle.Normal:
                    return null;
                case SkinStyle.Box:
                    return Style.skinBoxStyle;
                case SkinStyle.Round:
                    return Style.skinHelpStyle;
            }

            return Style.skinLabelStyle;
        }

        private static GUIContent GetContent(LabelAttribute attribute)
        {
            if (attribute.Content != null)
            {
                //try to find associated image content
                var content = EditorGUIUtility.TrIconContent(attribute.Content);
                if (content.image == null)
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, "Cannot find icon '" + attribute.Content + "'.");
                }
                content.text = attribute.Label;
                content.tooltip = string.Empty;
                return content;
            }
            else
            {
                return new GUIContent(attribute.Label);
            }
        }

        private static IDisposable CreateScopeIfNeeded(GUIStyle style)
        {
            return style != null ? new EditorGUILayout.VerticalScope(style) : null;
        }


        private static class Style
        {
            internal static readonly GUIStyle skinBoxStyle;
            internal static readonly GUIStyle skinHelpStyle;
            internal static readonly GUIStyle skinLabelStyle;

            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                //initialize optional scope styles
                skinBoxStyle = new GUIStyle("box");
                skinHelpStyle = new GUIStyle("helpBox");
                skinLabelStyle = new GUIStyle("label");

                //initialize optional label styles
                labelStyle = new GUIStyle(EditorStyles.label);
            }
        }
    }
}