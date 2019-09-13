namespace Toolbox.Editor.Drawers
{
    public class DisableAttributeDrawer : ToolboxAreaDrawer<DisableAttribute>
    {
        public override void OnGuiBegin(DisableAttribute attribute)
        {
            UnityEditor.EditorGUI.BeginDisabledGroup(true);
        }

        public override void OnGuiEnd(DisableAttribute attribute)
        {
            UnityEditor.EditorGUI.EndDisabledGroup();
        }
    }
}