namespace Toolbox.Editor.Drawers
{
    public class ShowIfToggle : ConditionalDrawer
    {
        public ShowIfToggle(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value.HasValue ? value.Value : false;
        }
    }
}