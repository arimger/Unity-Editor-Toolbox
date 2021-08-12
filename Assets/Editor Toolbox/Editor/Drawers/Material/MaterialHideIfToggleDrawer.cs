namespace Toolbox.Editor.Drawers
{
    public class MaterialHideIfToggleDrawer : MaterialConditionalDrawer
    {
        public MaterialHideIfToggleDrawer(string togglePropertyName) : base(togglePropertyName)
        { }


        protected override bool IsVisible(bool? value)
        {
            return value.HasValue && !value.Value;
        }
    }
}