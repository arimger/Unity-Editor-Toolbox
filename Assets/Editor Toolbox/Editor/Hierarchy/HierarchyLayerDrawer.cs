using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public class HierarchyLayerDrawer : HierarchyDataDrawer
    {
        public override void OnGui(Rect rect)
        {
            var layerMask = target.layer;
            var layerName = LayerMask.LayerToName(layerMask);

            string GetContentText()
            {
                switch (layerMask)
                {
                    //keep the default layer as an empty label
                    case 00: return string.Empty;
                    //for UI elements we can use the full name
                    case 05: return layerName;
                    default: return layerMask.ToString();
                }
            }

            var content = new GUIContent(GetContentText(), layerName + " layer");

            //draw label for the gameObject's specific layer
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
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleCenter
#else
                    alignment = TextAnchor.UpperCenter
#endif
                };
            }
        }
    }
}