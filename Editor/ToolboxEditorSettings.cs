using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    public interface IToolboxDrawersSettings
    {
        void AddAreaDrawerHandler(SerializedTypeReference drawerReference);
        void AddPropertyDrawerHandler(SerializedTypeReference drawerReference);
        void AddConditionDrawerHandler(SerializedTypeReference drawerReference);
        void AddCollectionDrawerHandler(SerializedTypeReference drawerReference);
        void AddTargetTypeDrawerHandler(SerializedTypeReference drawerReference);
        void RemoveAreaDrawerHandler(SerializedTypeReference drawerReference);
        void RemovePropertyDrawerHandler(SerializedTypeReference drawerReference);
        void RemoveConditionDrawerHandler(SerializedTypeReference drawerReference);
        void RemoveCollectionDrawerHandler(SerializedTypeReference drawerReference);
        void RemoveTargetTypeDrawerHandler(SerializedTypeReference drawerReference);

        Type GetAreaDrawerTypeAt(int index);
        Type GetPropertyDrawerTypeAt(int index);
        Type GetConditionDrawerTypeAt(int index);
        Type GetCollectionDrawerTypeAt(int index);
        Type GetTargetTypeDrawerTypeAt(int index);

        bool UseToolboxDrawers { get; }

        int AreaDrawersCount { get; }
        int PropertyDrawersCount { get; }
        int ConditionDrawersCount { get; }
        int CollectionDrawersCount { get; }
        int TargetTypeDrawersCount { get; }
    }

    public interface IToolboxProjectSettings
    {
        void AddCustomFolder(FolderData path);
        void RemoveCustomFolder(FolderData path);

        FolderData GetCustomFolderAt(int index);

        bool UseToolboxProject { get; }

        int CustomFoldersCount { get; }
    }

    public interface IToolboxHierarchySettings
    {
        bool UseToolboxHierarchy { get; }
    }

    [CreateAssetMenu(menuName = "Editor Toolbox/Settings", order = 1)]
    public class ToolboxEditorSettings : ScriptableObject, IToolboxHierarchySettings, IToolboxProjectSettings, IToolboxDrawersSettings
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

        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxAreaDrawer<>))]
        private List<SerializedTypeReference> areaDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxPropertyDrawer<>))]
        private List<SerializedTypeReference> propertyDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedTypeReference> conditionDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxCollectionDrawer<>))]
        private List<SerializedTypeReference> collectionDrawerHandlers;

        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxTargetTypeDrawer))]
        private List<SerializedTypeReference> targetTypeDrawerHandlers;


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

        public void AddTargetTypeDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (targetTypeDrawerHandlers == null) targetTypeDrawerHandlers = new List<SerializedTypeReference>();
            targetTypeDrawerHandlers.Add(drawerReference);
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

        public void RemoveTargetTypeDrawerHandler(SerializedTypeReference drawerReference)
        {
            targetTypeDrawerHandlers?.Remove(drawerReference);
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

        public Type GetTargetTypeDrawerTypeAt(int index)
        {
            return targetTypeDrawerHandlers[index].Type;
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

        public int TargetTypeDrawersCount => targetTypeDrawerHandlers != null ? targetTypeDrawerHandlers.Count : 0;
    }
}