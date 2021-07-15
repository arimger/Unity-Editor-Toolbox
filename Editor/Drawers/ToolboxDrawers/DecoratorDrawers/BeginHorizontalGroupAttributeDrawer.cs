using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalGroupAttribute>
    {
        static BeginHorizontalGroupAttributeDrawer()
        {
            storage = new ControlDataStorage<Vector2>((id, defaultValue) =>
            {
                return defaultValue;
            });
        }

        /// <summary>
        /// Storage used to cache scroll values depending on the given control ID.
        /// </summary>
        private static readonly ControlDataStorage<Vector2> storage;


        private void HandleScrollView(float fixedHeight)
        {
            var controlId = storage.GetControlId();
            var oldScroll = storage.ReturnItem(controlId, Vector2.zero);
            var newScroll = fixedHeight > 0.0f
                ? EditorGUILayout.BeginScrollView(oldScroll, Style.scrollViewGroupStyle, GUILayout.Height(fixedHeight))
                : EditorGUILayout.BeginScrollView(oldScroll, Style.scrollViewGroupStyle);
            storage.AppendItem(controlId, newScroll);
        }


        protected override void OnGuiBeginSafe(BeginHorizontalGroupAttribute attribute)
        {
            var fixedWidth = EditorGUIUtility.currentViewWidth;
            var fixedHeight = attribute.Height;
            EditorGUIUtility.labelWidth = fixedWidth * attribute.LabelToWidthRatio;
            EditorGUIUtility.fieldWidth = fixedWidth * attribute.FieldToWidthRatio;

            ToolboxLayoutHelper.BeginVertical(Style.groupBackgroundStyle);
            if (attribute.HasLabel)
            {
                GUILayout.Label(attribute.Label, EditorStyles.boldLabel);
            }

            HandleScrollView(fixedHeight);
            ToolboxLayoutHelper.BeginHorizontal();
        }


        private static class Style
        {
            internal static readonly GUIStyle groupBackgroundStyle;
            internal static readonly GUIStyle scrollViewGroupStyle;

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

                //NOTE: we need to add the right padding to keep foldout-based properties fully visible
                scrollViewGroupStyle = new GUIStyle("scrollView")
                {
                    padding = new RectOffset(13, 8, 2, 2)
                };
            }
        }
    }
}
