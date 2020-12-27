using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public class HierarchyIconDrawer : HierarchyDataDrawer
    {
        public override void OnGui(Rect rect)
        {
            var content = EditorGuiUtility.GetObjectContent(target, typeof(GameObject));
            if (content.image)
            {
                GUI.Label(rect, content.image);
            }
        }
    }
}