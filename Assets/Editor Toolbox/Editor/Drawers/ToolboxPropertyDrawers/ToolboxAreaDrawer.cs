namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxAreaDrawer<T> : ToolboxAreaDrawerBase where T : ToolboxAreaAttribute
    {        
        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public static System.Type GetAttributeType() => typeof(T);


        public override sealed void OnGuiBegin()
        {
            base.OnGuiBegin();
        }

        public override sealed void OnGuiBegin(ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGuiBegin(targetAttribute);
                return;
            }
            else
            {
                UnityEngine.Debug.LogError("Target attribute not found.");
            }

            base.OnGuiBegin();
        }

        public override sealed void OnGuiEnd()
        {
            base.OnGuiEnd();
        }

        public override sealed void OnGuiEnd(ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGuiEnd(targetAttribute);
                return;
            }
            else
            {
                UnityEngine.Debug.LogError("Target attribute not found.");
            }

            base.OnGuiEnd();
        }

        public virtual void OnGuiBegin(T attribute)
        {
            base.OnGuiBegin();
        }

        public virtual void OnGuiEnd(T attribute)
        {
            base.OnGuiEnd();
        }
    }
}