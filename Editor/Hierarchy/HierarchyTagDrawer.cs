using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public class HierarchyTagDrawer : HierarchyDataDrawer
    {
        public override float GetWidth()
        {
            return 60.0f;
        }

        public override void OnGui(Rect rect)
        {
            var content = new GUIContent(target.CompareTag("Untagged") ? string.Empty : target.tag, target.tag);
            EditorGUI.LabelField(rect, content, Style.labelStyle);
        }


        private static class Style
        {
            internal static readonly GUIStyle labelStyle;

            static Style()
            {
                labelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9
#else
                    fontSize = 8
#endif
                };
            }
        }
    }
}