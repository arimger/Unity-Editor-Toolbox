using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxTargetTypeDrawer : ToolboxDrawer
    {
        public abstract void OnGui(SerializedProperty property);

        public abstract System.Type GetTargetType();
    }
}