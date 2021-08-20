using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LabelAttributeDrawer : ToolboxDecoratorDrawer<LabelAttribute>
    {
        protected override void OnGuiBeginSafe(LabelAttribute attribute)
        {
            //prepare proper styles for the scope and text
            var scopeStyle = GetScopeStyle(attribute.SkinStyle);
            var labelStyle = GetLabelStyle(attribute.FontStyle);

            GUILayout.Space(attribute.SpaceBefore);
            //create (optionally) the vertical scope group
            using (CreateScopeIfNeeded(scopeStyle))
            {
                labelStyle.alignment = attribute.Alignment;
                labelStyle.fontStyle = attribute.FontStyle;
                EditorGUILayout.LabelField(GetContent(attribute), labelStyle);
            }

            GUILayout.Space(attribute.SpaceAfter);
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
                    return Style.boxedScopeStyle;
                case SkinStyle.Round:
                    return Style.roundScopeStyle;
            }

            return Style.labelScopeStyle;
        }

        private static GUIContent GetContent(LabelAttribute attribute)
        {
            if (attribute.Asset != null)
            {
                //try to find associated image content
                var content = EditorGUIUtility.TrIconContent(attribute.Asset);
                if (content.image == null)
                {
                    ToolboxEditorLog.AttributeUsageWarning(attribute, string.Format("Cannot find icon asset '{0}'.", attribute.Asset));
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
            internal static readonly GUIStyle boxedScopeStyle;
            internal static readonly GUIStyle roundScopeStyle;
            internal static readonly GUIStyle labelScopeStyle;

            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                //initialize possible scope styles
                boxedScopeStyle = new GUIStyle("box");
                roundScopeStyle = new GUIStyle("helpBox");
                labelScopeStyle = new GUIStyle("label");

                //initialize default label style
                labelStyle = new GUIStyle(EditorStyles.label)
                {
                    richText = true
                };
            }
        }
    }
}