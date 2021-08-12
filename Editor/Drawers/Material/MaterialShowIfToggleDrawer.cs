namespace Toolbox.Editor.Drawers
{
    public class MaterialShowIfToggleDrawer : MaterialConditionalDrawer
    {
        public MaterialShowIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value ?? false;
        }
    }
}