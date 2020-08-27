using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public class LabelAttributeDrawer : ToolboxDecoratorDrawer<LabelAttribute>
    {
        protected override void OnGuiBeginSafe(LabelAttribute attribute)
        {
            var spaceStyle = GetSpaceStyle(attribute.SkinStyle);
            var labelStyle = GetLabelStyle(attribute.FontStyle);

            labelStyle.alignment = attribute.Alignment;
            labelStyle.fontStyle = attribute.FontStyle;

            var content = GetContent(attribute);

            EditorGUILayout.BeginVertical(spaceStyle);
            EditorGUILayout.LabelField(content, labelStyle);
            EditorGUILayout.EndVertical();
        }


        private static GUIStyle GetLabelStyle(FontStyle style)
        {
            return Style.labelStyle;
        }

        private static GUIStyle GetSpaceStyle(SkinStyle style)
        {
            switch (style)
            {
                case SkinStyle.Normal:
                    return Style.skinLabelStyle;
                case SkinStyle.Box:
                    return Style.skinBoxStyle;
                case SkinStyle.Help:
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
            internal static readonly GUIStyle skinLabelStyle;
            internal static readonly GUIStyle skinBoxStyle;
            internal static readonly GUIStyle skinHelpStyle;

            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                skinLabelStyle = new GUIStyle("label");
                skinBoxStyle   = new GUIStyle("box");
                skinHelpStyle  = new GUIStyle("helpBox");

                labelStyle = new GUIStyle(EditorStyles.label);
            }
        }
    }
}