using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    [CreateAssetMenu(menuName = "Editor Toolbox/Settings", order = 1)]
    public class ToolboxEditorSettings : ScriptableObject
    {
        [SerializeField]
        private bool useToolboxHierarchy = true;
        [SerializeField]
        private bool useToolboxFolders = true;
        [SerializeField]
        private bool useToolboxDrawers = true;

        [HideIf("useToolboxProject", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed)]
        private List<FolderData> customFolders;

        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxAreaDrawer<>))]
        private List<SerializedTypeReference> areaDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxPropertyDrawer<>))]
        private List<SerializedTypeReference> propertyDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedTypeReference> conditionDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxCollectionDrawer<>))]
        private List<SerializedTypeReference> collectionDrawerHandlers;


        public void AddCustomFolder(FolderData path)
        {
            if (customFolders == null) customFolders = new List<FolderData>();
            customFolders.Add(path);
        }

        public void RemoveCustomFolder(FolderData path)
        {
            customFolders?.Remove(path);
        }

        public FolderData GetCustomFolderAt(int index)
        {
            return customFolders[index];
        }


        public void AddAreaDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (areaDrawerHandlers == null) areaDrawerHandlers = new List<SerializedTypeReference>();
            areaDrawerHandlers.Add(drawerReference);
        }

        public void AddPropertyDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (propertyDrawerHandlers == null) propertyDrawerHandlers = new List<SerializedTypeReference>();
            propertyDrawerHandlers.Add(drawerReference);
        }

        public void AddConditionDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (conditionDrawerHandlers == null) conditionDrawerHandlers = new List<SerializedTypeReference>();
            conditionDrawerHandlers.Add(drawerReference);
        }

        public void AddCollectionDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (conditionDrawerHandlers == null) collectionDrawerHandlers = new List<SerializedTypeReference>();
            collectionDrawerHandlers.Add(drawerReference);
        }

        public void RemoveAreaDrawerHandler(SerializedTypeReference drawerReference)
        {
            areaDrawerHandlers?.Remove(drawerReference);
        }

        public void RemovePropertyDrawerHandler(SerializedTypeReference drawerReference)
        {
            propertyDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveConditionDrawerHandler(SerializedTypeReference drawerReference)
        {
            conditionDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveCollectionDrawerHandler(SerializedTypeReference drawerReference)
        {
            collectionDrawerHandlers?.Remove(drawerReference);
        }

        public Type GetAreaDrawerTypeAt(int index)
        {
            return areaDrawerHandlers[index];
        }

        public Type GetPropertyDrawerTypeAt(int index)
        {
            return propertyDrawerHandlers[index].Type;
        }

        public Type GetConditionDrawerTypeAt(int index)
        {
            return conditionDrawerHandlers[index].Type;
        }

        public Type GetCollectionDrawerTypeAt(int index)
        {
            return collectionDrawerHandlers[index].Type;
        }


        public bool UseToolboxHierarchy
        {
            get => useToolboxHierarchy;
            set => useToolboxHierarchy = value;
        }

        public bool UseToolboxProject
        {
            get => useToolboxFolders;
            set => useToolboxFolders = value;
        }

        public bool UseToolboxDrawers
        {
            get => useToolboxDrawers;
            set => useToolboxDrawers = value;
        }


        public int CustomFoldersCount => customFolders != null ? customFolders.Count : 0;


        public int AreaDrawersCount => areaDrawerHandlers != null ? areaDrawerHandlers.Count : 0;

        public int PropertyDrawersCount => propertyDrawerHandlers != null ? propertyDrawerHandlers.Count : 0;

        public int ConditionDrawersCount => conditionDrawerHandlers != null ? conditionDrawerHandlers.Count : 0;

        public int CollectionDrawersCount => collectionDrawerHandlers != null ? collectionDrawerHandlers.Count : 0;
    }
}