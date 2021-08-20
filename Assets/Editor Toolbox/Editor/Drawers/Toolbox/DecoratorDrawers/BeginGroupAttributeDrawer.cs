using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginGroupAttribute>
    {
        protected override void OnGuiBeginSafe(BeginGroupAttribute attribute)
        {
            ToolboxLayoutHelper.BeginVertical(Style.groupBackgroundStyle);
            if (attribute.HasLabel)
            {
                GUILayout.Label(attribute.Label, EditorStyles.boldLabel);
            }
        }


        private static class Style
        {
            internal static readonly GUIStyle groupBackgroundStyle;

            static Style()
            {
#if UNITY_2019_3_OR_NEWER
                groupBackgroundStyle = new GUIStyle("helpBox")
#else
                groupBackgroundStyle = new GUIStyle("box")
#endif
                {
                    padding = new RectOffset(13, 12, 5, 5)
                };
            }
        }
    }
}