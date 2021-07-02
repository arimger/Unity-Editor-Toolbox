using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class IgnoreParentAttributeDrawer : ToolboxSelfPropertyDrawer<IgnoreParentAttribute>
    {
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, IgnoreParentAttribute attribute)
        {
            var targetProperty = property.Copy();
            targetProperty.NextVisible(true);
            //draw first child, right after the parent property
            ToolboxEditorGui.DrawToolboxProperty(targetProperty.Copy());
            var targetDepth = targetProperty.depth;
            while (targetProperty.NextVisible(false))
            {
                if (targetProperty.depth > targetDepth)
                {
                    break;
                }

                //draw all children in order but only one level depth
                ToolboxEditorGui.DrawToolboxProperty(targetProperty.Copy());
            }
        }
    }
}