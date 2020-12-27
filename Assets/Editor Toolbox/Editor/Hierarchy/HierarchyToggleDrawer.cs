using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public class HierarchyToggleDrawer : HierarchyDataDrawer
    {
        public override void OnGui(Rect rect)
        {
            var content = new GUIContent(string.Empty, "Enable/disable GameObject");
            //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
            var result = GUI.Toggle(new Rect(rect.x + EditorGUIUtility.standardVerticalSpacing,
                                             rect.y,
                                             rect.width,
                                             rect.height),
                                             target.activeSelf, content);

            if (rect.Contains(Event.current.mousePosition))
            {
                if (result != target.activeSelf)
                {
                    Undo.RecordObject(target, "SetActive");
                    target.SetActive(result);
                }
            }
        }
    }
}