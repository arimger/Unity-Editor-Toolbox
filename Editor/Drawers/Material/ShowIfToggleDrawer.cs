namespace Toolbox.Editor.Drawers
{
    public class ShowIfToggleDrawer : ConditionalDrawer
    {
        public ShowIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value ?? false;
        }
    }
}