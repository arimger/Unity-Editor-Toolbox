using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Base class for all reorderable list related implementations.
    /// </summary>
    public abstract class ReorderableListBase
    {
        public delegate void DrawRectCallbackDelegate(Rect rect);

        public delegate bool CanChangeListCallbackDelegate(ReorderableList list);
        public delegate void ChangeDetailsCallbackDelegate(ReorderableList list, int oldIndex, int newIndex);
        public delegate void ChangeListNowCallbackDelegate(ReorderableList list);

        public delegate void DrawRelatedRectCallbackDelegate(Rect rect, ReorderableList list);


        protected const string defaultLabelFormat = "{0} {1}";

        /// <summary>
        /// Hotcontrol index, unique for this instance.
        /// </summary>
        protected readonly int id = -1;


        public ReorderableListBase(SerializedProperty list) : this(list, null, true, true, false)
        { }

        public ReorderableListBase(SerializedProperty list, bool draggable) : this(list, null, draggable, true, false)
        { }

        public ReorderableListBase(SerializedProperty list, string elementLabel, bool draggable, bool hasHeader, bool fixedSize)
        {
            //validate parameters
            if (list == null || list.isArray == false)
            {
                throw new ArgumentException("List should be an Array SerializedProperty.", nameof(list));
            }

            id = GetHashCode();

            //set basic properties
            Draggable = draggable;
            HasHeader = hasHeader;
            FixedSize = fixedSize;
            //set other properties
            ElementLabel = elementLabel;

            //ser serialized data
            List = list;
            Size = list.FindPropertyRelative("Array.size");
        }


        public string GetElementDefaultName(int index)
        {
            return string.Format(defaultLabelFormat, "Element", index);
        }

        public string GetElementDefinedName(int index)
        {
            return ElementLabel != null
                ? string.Format(defaultLabelFormat, ElementLabel, index) : null;
        }

        public string GetElementDisplayName(SerializedProperty element, int index)
        {
            //try to determine name using the internal API
            var elementName = element.displayName;
            var defaultName = GetElementDefaultName(index);
            if (defaultName != elementName)
            {
                return elementName;
            }

            //try to override name using customized label
            var definedName = GetElementDefinedName(index);
            if (definedName == null)
            {
                return elementName;
            }

            return definedName;
        }

        public void SetKeyboardFocus()
        {
            GUIUtility.keyboardControl = id;
        }

        public bool HasKeyboardFocus()
        {
            return GUIUtility.keyboardControl == id;
        }

        public void CutKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        public void AppendElement()
        {
            Index = (List.arraySize += 1) - 1;
        }

        public void RemoveElement()
        {
            RemoveElement(Index);
        }

        public void RemoveElement(int index)
        {
            List.DeleteArrayElementAtIndex(index);
            if (Index >= List.arraySize - 1)
            {
                Index = List.arraySize - 1;
            }
        }


        /// <summary>
        /// Index of the currently active element.
        /// </summary>
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

                //if we are during multi-selection
                var smallerArraySize = List.arraySize;
                foreach (var targetObject in List.serializedObject.targetObjects)
                {
                    using (var serializedObject = new SerializedObject(targetObject))
                    {
                        var property = serializedObject.FindProperty(List.propertyPath);
                        smallerArraySize = Math.Min(property.arraySize, smallerArraySize);
                    }
                }
                //return the smallest array size
                return smallerArraySize;
            }
        }

        public bool Draggable
        {
            get; set;
        }

        public bool IsDragging
        {
            get; protected set;
        }

        public bool FixedSize
        {
            get; protected set;
        }

        public bool HasHeader
        {
            get; set;
        }

        public bool HasLabels
        {
            get; set;
        } = true;

        public float HeaderHeight { get; set; } = 18.0f;

        public float FooterHeight { get; set; } = 20.0f;

        /// <summary>
        /// Standard spacing between elements.
        /// </summary>
        public float ElementSpacing
        {
            get; set;
        } = 5;

        /// <summary>
        /// Custom element label name.
        /// "{<see cref="ElementLabel"/>} {index}"
        /// </summary>
        public string ElementLabel
        {
            get; set;
        }

        /// <summary>
        /// Child Array.size property.
        /// </summary>
        public SerializedProperty Size
        {
            get; protected set;
        }

        /// <summary>
        /// Associated array property.
        /// </summary>
        public SerializedProperty List
        {
            get; protected set;
        }
    }
}