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
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedPresetDrawer<>))]
        private List<SerializedTypeReference> presetHandlers;
        [DrawIf("useOrderedDrawers", true)]
        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedPropertyDrawer<>))]
        private List<SerializedTypeReference> propertyHandlers;


        public void AddPresetHandler(SerializedTypeReference attributeHandler)
        {
            if (presetHandlers == null) presetHandlers = new List<SerializedTypeReference>();
            presetHandlers.Add(attributeHandler);
        }

        public void RemovePresetHandler(SerializedTypeReference attributeHandler)
        {
            presetHandlers?.Remove(attributeHandler);
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

        public SerializedTypeReference GetPresetHandlerAt(int index)
        {
            return presetHandlers[index];
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

        public int PresetHandlersCount => presetHandlers != null ? presetHandlers.Count : 0;

        public int PropertyHandlersCount => propertyHandlers != null ? propertyHandlers.Count : 0;
    }
}