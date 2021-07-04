using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class BeginHorizontalGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginHorizontalGroupAttribute>
    {
        static BeginHorizontalGroupAttributeDrawer()
        {
            storage = new ControlDataStorage<Vector2>((i, defaultValue) => defaultValue);
        }

        /// <summary>
        /// Storage used to cache scroll values depending on the given control ID.
        /// </summary>
        private static readonly ControlDataStorage<Vector2> storage;


        private void HandleScrollView()
        {
            var controlId = storage.GetControlId();
            var oldScroll = storage.ReturnItem(controlId, Vector2.zero);
            var newScroll = EditorGUILayout.BeginScrollView(oldScroll);
            storage.AppendItem(controlId, newScroll);
        }

        private void AdjustLeftMargin()
        {
            EditorGUILayout.Space(Style.extraLeftPadding);
        }


        protected override void OnGuiBeginSafe(BeginHorizontalGroupAttribute attribute)
        {
            var width = EditorGUIUtility.currentViewWidth;
            EditorGUIUtility.labelWidth = width * attribute.LabelToWidthRatio;
            EditorGUIUtility.fieldWidth = width * attribute.FieldToWidthRatio;

            ToolboxLayoutHelper.BeginVertical(Style.groupBackgroundStyle);
            if (attribute.HasLabel)
            {
                GUILayout.Label(attribute.Label, EditorStyles.boldLabel);
            }

            HandleScrollView();
            ToolboxLayoutHelper.BeginHorizontal();
            AdjustLeftMargin();
        }


        private static class Style
        {
            /// <summary>
            /// Additional padding applied to keep foldout-based labels within the group.
            /// </summary>
            internal static readonly float extraLeftPadding = 8.0f;

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
