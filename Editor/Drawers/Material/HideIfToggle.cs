namespace Toolbox.Editor.Drawers
{
    public class HideIfToggle : ConditionalDrawer
    {
        public HideIfToggle(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value.HasValue ? !value.Value : false;
        }
    }
}