namespace Toolbox.Editor.Drawers
{
    public class HideIfToggleDrawer : ConditionalDrawer
    {
        public HideIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value.HasValue && !value.Value;
        }
    }
}