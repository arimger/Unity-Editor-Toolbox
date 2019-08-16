using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor
{
    [CreateAssetMenu(menuName = "Editor Toolbox/Settings", order = 1)]
    public class ComponentEditorSettings : ScriptableObject
    {
        [SerializeField]
        private bool useOrderedDrawers = true;

        [DrawIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedGroupDrawer<>))]
        private List<SerializedTypeReference> groupHandlers;
        [DrawIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedDecoratorDrawer<>))]
        private List<SerializedTypeReference> decoratorHandlers;
        [DrawIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedPropertyDrawer<>))]
        private List<SerializedTypeReference> propertyHandlers;


        public void AddGroupHandler(SerializedTypeReference attributeHandler)
        {
            if (groupHandlers == null) groupHandlers = new List<SerializedTypeReference>();
            groupHandlers.Add(attributeHandler);
        }

        public void RemoveGroupHandler(SerializedTypeReference attributeHandler)
        {
            groupHandlers?.Remove(attributeHandler);
        }

        public void AddDecoratorHandler(SerializedTypeReference attributeHandler)
        {
            if (decoratorHandlers == null) decoratorHandlers = new List<SerializedTypeReference>();
            decoratorHandlers.Add(attributeHandler);
        }

        public void RemoveDecoratorHandler(SerializedTypeReference attributeHandler)
        {
            decoratorHandlers?.Remove(attributeHandler);
        }

        public void AddPropertyHandler(SerializedTypeReference attributeHandler)
        {
            if (propertyHandlers == null) propertyHandlers = new List<SerializedTypeReference>();
            propertyHandlers.Add(attributeHandler);
        }

        public void RemovePropertyHandler(SerializedTypeReference attributeHandler)
        {
            propertyHandlers?.Remove(attributeHandler);
        }

        public SerializedTypeReference GetGroupHandlerAt(int index)
        {
            return groupHandlers[index];
        }

        public SerializedTypeReference GetDecoratorHandlerAt(int index)
        {
            return decoratorHandlers[index];
        }

        public SerializedTypeReference GetPropertyHandlerAt(int index)
        {
            return propertyHandlers[index];
        }


        public bool UseOrderedDrawers
        {
            get => useOrderedDrawers;
            set => useOrderedDrawers = value;
        }

        public int GroupHandlersCount => groupHandlers != null ? groupHandlers.Count : 0;

        public int DecoratorHandlersCount => decoratorHandlers != null ? decoratorHandlers.Count : 0;

        public int PropertyHandlersCount => propertyHandlers != null ? propertyHandlers.Count : 0;
    }
}