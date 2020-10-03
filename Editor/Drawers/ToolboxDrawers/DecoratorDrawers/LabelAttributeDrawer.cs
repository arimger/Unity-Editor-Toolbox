using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class LabelAttributeDrawer : ToolboxDecoratorDrawer<LabelAttribute>
    {
        protected override void OnGuiBeginSafe(LabelAttribute attribute)
        {
            var scopeStyle = GetScopeStyle(attribute.SkinStyle);
            var labelStyle = GetLabelStyle(attribute.FontStyle);

            labelStyle.alignment = attribute.Alignment;
            labelStyle.fontStyle = attribute.FontStyle;

            using (new EditorGUILayout.VerticalScope(scopeStyle))
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
                    return Style.skinLabelStyle;
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


        private static class Style
        {
            internal static readonly GUIStyle skinBoxStyle;
            internal static readonly GUIStyle skinHelpStyle;
            internal static readonly GUIStyle skinLabelStyle;

            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                skinBoxStyle = new GUIStyle("box");
                skinHelpStyle = new GUIStyle("helpBox");
                skinLabelStyle = new GUIStyle("label");

                labelStyle = new GUIStyle(EditorStyles.label);
            }
        }
    }
}