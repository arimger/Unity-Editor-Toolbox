using System;
using System.Collections.Generic;

namespace Toolbox.Editor.Drawers
{
    internal static class DrawerStorageManager
    {
        static DrawerStorageManager()
        {
            ToolboxEditorHandler.OnEditorReload -= ClearStorages;
            ToolboxEditorHandler.OnEditorReload += ClearStorages;
        }

        private static readonly List<DrawerDataStorageBase> storages = new List<DrawerDataStorageBase>();

        internal static void ClearStorages()
        {
            ClearStorages(null);
        }

        internal static void ClearStorages(Func<DrawerDataStorageBase, bool> predicate)
        {
            for (var i = 0; i < storages.Count; i++)
            {
                var storage = storages[i];
                if (predicate != null && !predicate(storage))
                {
                    continue;
                }

                storage.ClearItems();
            }
        }

        internal static void AppendStorage(DrawerDataStorageBase storage)
        {
            storages.Add(storage);
        }

        internal static bool RemoveStorage(DrawerDataStorageBase storage)
        {
            return storages.Remove(storage);
        }
    }
}
