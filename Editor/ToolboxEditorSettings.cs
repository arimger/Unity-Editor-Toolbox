using System;
using System.Linq;
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
        private bool useToolboxDrawers = true;

        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxAreaDrawer<>))]
        private List<SerializedTypeReference> areaDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxGroupDrawer<>))]
        private List<SerializedTypeReference> groupDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxPropertyDrawer<>))]
        private List<SerializedTypeReference> propertyDrawerHandlers;
        [HideIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(ToolboxConditionDrawer<>))]
        private List<SerializedTypeReference> conditionDrawerHandlers;


        public void AddAreaDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (areaDrawerHandlers == null) areaDrawerHandlers = new List<SerializedTypeReference>();
            areaDrawerHandlers.Add(drawerReference);
        }

        public void AddGroupDrawerHandler(SerializedTypeReference drawerReference)
        {
            if (groupDrawerHandlers == null) groupDrawerHandlers = new List<SerializedTypeReference>();
            groupDrawerHandlers.Add(drawerReference);
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

        public void RemoveAreaDrawerHandler(SerializedTypeReference drawerReference)
        {
            areaDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveGroupDrawerHandler(SerializedTypeReference drawerReference)
        {
            groupDrawerHandlers?.Remove(drawerReference);
        }

        public void RemovePropertyDrawerHandler(SerializedTypeReference drawerReference)
        {
            propertyDrawerHandlers?.Remove(drawerReference);
        }

        public void RemoveConditionDrawerHandler(SerializedTypeReference drawerReference)
        {
            conditionDrawerHandlers?.Remove(drawerReference);
        }

        public Type GetAreaDrawerTypeAt(int index)
        {
            return areaDrawerHandlers[index];
        }

        public Type GetGroupDrawerTypeAt(int index)
        {
            return groupDrawerHandlers[index];
        }

        public Type GetPropertyDrawerTypeAt(int index)
        {
            return propertyDrawerHandlers[index].Type;
        }

        public Type GetConditionDrawerTypeAt(int index)
        {
            return conditionDrawerHandlers[index].Type;
        }


        public bool UseToolboxDrawers
        {
            get => useToolboxDrawers;
            set => useToolboxDrawers = value;
        }

        public bool UseToolboxHierarchy
        {
            get => useToolboxHierarchy;
            set => useToolboxHierarchy = value;
        }

        public int AreaDrawersCount => areaDrawerHandlers != null ? areaDrawerHandlers.Count : 0;

        public int GroupDrawersCount => groupDrawerHandlers != null ? groupDrawerHandlers.Count : 0;

        public int PropertyDrawersCount => propertyDrawerHandlers != null ? propertyDrawerHandlers.Count : 0;

        public int ConditionDrawersCount => conditionDrawerHandlers != null ? conditionDrawerHandlers.Count : 0;
    }
}