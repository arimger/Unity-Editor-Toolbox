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

        [SerializeField, ReorderableList(ListStyle.Boxed), ClassExtends(typeof(OrderedPropertyDrawerRoot))]
        private List<SerializedTypeReference> drawHandlers;


        public void AddAttributeHandler(SerializedTypeReference attributeHandler)
        {
            if (drawHandlers == null) drawHandlers = new List<SerializedTypeReference>();
            drawHandlers.Add(attributeHandler);
        }

        public void RemoveAttributeHandler(SerializedTypeReference attributeHandler)
        {
            drawHandlers?.Remove(attributeHandler);
        }

        public SerializedTypeReference GetHandlerAt(int index)
        {
            return drawHandlers[index];
        }


        public bool UseOrderedDrawers
        {
            get => useOrderedDrawers;
            set => useOrderedDrawers = value;
        }

        public int HandlersCount => drawHandlers != null ? drawHandlers.Count : 0;
    }
}