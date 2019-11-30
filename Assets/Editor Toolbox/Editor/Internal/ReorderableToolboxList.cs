using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Variation of <see cref="ReorderableList"/> which is full layout-based and supports Toolbox Drawers.
    /// </summary>
    public class ReorderableToolboxList
    {        
        private readonly int id = -1;

        //private float draggedY;
        //private float dragOffset;

        private float headerHeight = 18.0f;
        private float footerHeight = 20.0f;

        //private List<int> nonDragTargetIndices;


        public ReorderableToolboxList(SerializedProperty list) : this(list, null, true, true, false)
        { }

        public ReorderableToolboxList(SerializedProperty list, bool draggable) : this(list, null, draggable, true, false)
        { }

        public ReorderableToolboxList(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool hasFixedSize)
        {
            id = GetHashCode();

            Draggable = draggable;
            HasHeader = hasHeader;
            HasFixedSize = hasFixedSize;
            ElementLabel = elementLabel;

            List = list;

            if (List != null && List.editable == false) Draggable = false;
            if (List != null && List.isArray == false)
            {
                throw new ArgumentException("Input elements should be an Array SerializedProperty.");
            }
        }


        public void DoLayout()
        {
            var headerRect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            var middleRect = GUILayoutUtility.GetRect(0, MiddleHeight, GUILayout.ExpandWidth(true));
            var footerRect = GUILayoutUtility.GetRect(0, FooterHeight, GUILayout.ExpandWidth(true));

            throw new NotImplementedException();
        }


        public int Index
        {
            get; set;
        } = -1;

        public int Count
        {
            get
            {
                if (!List.hasMultipleDifferentValues)
                {
                    return List.arraySize;
                }

                //if we are handling multi-selection
                var smallerArraySize = List.arraySize;
                foreach (var targetObject in List.serializedObject.targetObjects)
                {
                    var serializedObject = new SerializedObject(targetObject);
                    var property = serializedObject.FindProperty(List.propertyPath);
                    smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
                }
                //return smalest array size
                return smallerArraySize;
            }
        }


        public bool Draggable
        {
            get; set;
        }

        public bool IsDragging
        {
            get; private set;
        }

        public bool HasFixedSize
        {
            get; private set;
        }

        public bool HasHeader
        {
            get; set;
        }

        public float HeaderHeight
        {
            get => HasHeader ? headerHeight : 0.0f;
            set => headerHeight = value;
        }

        public float MiddleHeight
        {
            get => 0; //GetListElementHeight();
        }

        public float FooterHeight
        {
            get => HasFixedSize ? 0.0f : footerHeight;
            set => footerHeight = value;
        }

        /// <summary>
        /// Standard spacing between elements.
        /// </summary>
        public float ElementSpacing
        {
            get; set;
        } = 5;

        /// <summary>
        /// Standard element label name.
        /// </summary>
        public string ElementLabel
        {
            get; set;
        } = "Element";

        /// <summary>
        /// Serialized Array.size property.
        /// </summary>
        public SerializedProperty Size
        {
            get => List.FindPropertyRelative("Array.size");
        }

        /// <summary>
        /// Associated list property.
        /// </summary>
        public SerializedProperty List
        {
            get; private set;
        }
    }
}