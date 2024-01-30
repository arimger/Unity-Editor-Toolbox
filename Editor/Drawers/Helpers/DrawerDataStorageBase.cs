namespace Toolbox.Editor.Drawers
{
    public abstract class DrawerDataStorageBase
    {
        protected DrawerDataStorageBase()
        {
            DrawerStorageManager.AppendStorage(this);
        }

        ~DrawerDataStorageBase()
        {
            DrawerStorageManager.RemoveStorage(this);
        }

        /// <summary>
        /// Called each time Editor is completely destroyed.
        /// </summary>
        public abstract void ClearItems();
    }
}