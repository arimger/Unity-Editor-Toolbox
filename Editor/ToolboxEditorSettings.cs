using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Drawers;

    public interface IToolboxDrawersSettings
    {
        void SetAllPossibleDecoratorDrawers();
        void SetAllPossiblePropertyDrawers();
        void SetAllPossibleCollectionDrawers();
        void SetAllPossibleConditionDrawers();
        void SetAllPossibleTargetTypeDrawers();

        void AddDecoratorDrawerHandler(SerializedType drawerReference);
        void AddPropertyDrawerHandler(SerializedType drawerReference);
        void AddConditionDrawerHandler(SerializedType drawerReference);
        void AddCollectionDrawerHandler(SerializedType drawerReference);
        void AddTargetTypeDrawerHandler(SerializedType drawerReference);
        void RemoveDecoratorDrawerHandler(SerializedType drawerReference);
        void RemovePropertyDrawerHandler(SerializedType drawerReference);
        void RemoveConditionDrawerHandler(SerializedType drawerReference);
        void RemoveCollectionDrawerHandler(SerializedType drawerReference);
        void RemoveTargetTypeDrawerHandler(SerializedType drawerReference);

        Type GetDecoratorDrawerTypeAt(int index);
        Type GetPropertyDrawerTypeAt(int index);
        Type GetConditionDrawerTypeAt(int index);
        Type GetCollectionDrawerTypeAt(int index);
        Type GetTargetTypeDrawerTypeAt(int index);

        bool UseToolboxDrawers { get; }

        int DecoratorDrawersCount { get; }
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
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxDecoratorDrawer<>))]
        private List<SerializedType> decoratorDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxPropertyDrawer<>))]
        private List<SerializedType> propertyDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxCollectionDrawer<>))]
        private List<SerializedType> collectionDrawerHandlers;
        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedType> conditionDrawerHandlers;

        [HideIf("useToolboxDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxTargetTypeDrawer))]
        private List<SerializedType> targetTypeDrawerHandlers;


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


        public void SetAllPossibleDecoratorDrawers()
        {
            decoratorDrawerHandlers?.Clear();

            var types = ToolboxDrawerUtility.GetAllPossibleDecoratorDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddDecoratorDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossiblePropertyDrawers()
        {
            propertyDrawerHandlers?.Clear();

            var types = ToolboxDrawerUtility.GetAllPossiblePropertyDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddPropertyDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleCollectionDrawers()
        {
            collectionDrawerHandlers?.Clear();

            var types = ToolboxDrawerUtility.GetAllPossibleCollectionDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddCollectionDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleConditionDrawers()
        {
            conditionDrawerHandlers?.Clear();

            var types = ToolboxDrawerUtility.GetAllPossibleConditionDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddConditionDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void SetAllPossibleTargetTypeDrawers()
        {
            targetTypeDrawerHandlers?.Clear();

            var types = ToolboxDrawerUtility.GetAllPossibleTargetTypeDrawers();
            for (var i = 0; i < types.Count; i++)
            {
                AddTargetTypeDrawerHandler(new SerializedType(types[i]));
            }
        }

        public void AddDecoratorDrawerHandler(SerializedType drawerReference)
        {
            if (decoratorDrawerHandlers == null) decoratorDrawerHandlers = new List<SerializedType>();
            decoratorDrawerHandlers.Add(drawerReference);
        }

        public void AddPropertyDrawerHandler(SerializedType drawerReference)
        {
            if (propertyDrawerHandlers == null) propertyDrawerHandlers = new List<SerializedType>();
            propertyDrawerHandlers.Add(drawerReference);
        }

        public void AddConditionDrawerHandler(SerializedType drawerReference)
        {
            if (conditionDrawerHandlers == null) conditionDrawerHandlers = new List<SerializedType>();
            conditionDrawerHandlers.Add(drawerReference);
        }

        public void AddCollectionDrawerHandler(SerializedType drawerReference)
        {
            if (conditionDrawerHandlers == null) collectionDrawerHandlers = new List<SerializedType>();
            collectionDrawerHandlers.Add(drawerReference);
        }

        public void AddTargetTypeDrawerHandler(SerializedType drawerReference)
        {
            if (targetTypeDrawerHandlers == null) targetTypeDrawerHandlers = new List<SerializedType>();
            targetTypeDrawerHandlers.Add(drawerReference);
        }

        public void RemoveDecoratorDrawerHandler(SerializedType drawerReference)
        {
            decoratorDrawerHandlers?.Remove(drawerReference);
        }

        public void RemovePropertyDrawerHandler(SerializedType drawerReference)
        {
            propertyDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveConditionDrawerHandler(SerializedType drawerReference)
        {
            conditionDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveCollectionDrawerHandler(SerializedType drawerReference)
        {
            collectionDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveTargetTypeDrawerHandler(SerializedType drawerReference)
        {
            targetTypeDrawerHandlers?.Remove(drawerReference);
        }

        public Type GetDecoratorDrawerTypeAt(int index)
        {
            return decoratorDrawerHandlers[index];
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


        public int DecoratorDrawersCount => decoratorDrawerHandlers != null ? decoratorDrawerHandlers.Count : 0;

        public int PropertyDrawersCount => propertyDrawerHandlers != null ? propertyDrawerHandlers.Count : 0;

        public int ConditionDrawersCount => conditionDrawerHandlers != null ? conditionDrawerHandlers.Count : 0;

        public int CollectionDrawersCount => collectionDrawerHandlers != null ? collectionDrawerHandlers.Count : 0;

        public int TargetTypeDrawersCount => targetTypeDrawerHandlers != null ? targetTypeDrawerHandlers.Count : 0;
    }
}