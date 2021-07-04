using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    public abstract class DrawerDataStorageBase
    {
        static DrawerDataStorageBase()
        {
            InspectorUtility.OnEditorReload += () =>
            {
                for (var i = 0; i < storages.Count; i++)
                {
                    storages[i].ClearItems();
                }
            };
        }


        protected DrawerDataStorageBase()
        {
            storages.Add(this);
        }

        ~DrawerDataStorageBase()
        {
            storages.Remove(this);
        }


        private static readonly List<DrawerDataStorageBase> storages = new List<DrawerDataStorageBase>();


        /// <summary>
        /// Called each time Editor is completely destroyed.
        /// </summary>
        public abstract void ClearItems();
    }
}