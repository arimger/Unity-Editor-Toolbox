using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class IgnoreParentAttributeDrawer : ToolboxSelfPropertyDrawer<IgnoreParentAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, IgnoreParentAttribute attribute)
        {
            if (!property.hasVisibleChildren)
            {
                ToolboxEditorGui.DrawNativeProperty(property, label);
                return;
            }

            ToolboxEditorGui.DrawPropertyChildren(property);
        }
    }
}