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
        /// 
        /// Basic delegates used in custom list organization.
        ///

        public delegate float ElementCallbackDelegate(int index);

        public delegate void DrawRectCallbackDelegate(float height);

        public delegate void DrawIndexedRectCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);

        public delegate void DrawDetailedRectCallbackDelegate(Rect buttonRect, ReorderableToolboxList list);

        public delegate void ChangeListCallbackDelegate(ReorderableToolboxList list);

        public delegate bool CanChangeListCallbackDelegate(ReorderableToolboxList list);

        public delegate void ChangeDetailsCallbackDelegate(ReorderableToolboxList list, int oldIndex, int newIndex);

        public delegate bool CanChangeDetailsCallbackDelegate(ReorderableToolboxList list, int oldIndex, int newIndex);

        /// 
        /// All needed and provided draw callbacks
        ///

        public DrawRectCallbackDelegate drawHeaderBackgroundCallback = null;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback = null;
        public DrawRectCallbackDelegate drawMiddleBackgroundCallback = null;

        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawVoidedCallback = null;
        public DrawRectCallbackDelegate drawFooterCallback = null;

        public DrawIndexedRectCallbackDelegate drawHandleCallback = null;
        public DrawIndexedRectCallbackDelegate drawElementCallback;
        public DrawIndexedRectCallbackDelegate drawElementBackgroundCallback;

        public DrawDetailedRectCallbackDelegate onAddDropdownCallback;

        /// 
        /// All needed and provided interaction callbacks
        ///

        public ChangeListCallbackDelegate onAddCallback;
        public ChangeListCallbackDelegate onSelectCallback;
        public ChangeListCallbackDelegate onChangedCallback;
        public ChangeListCallbackDelegate onRemoveCallback;
        public ChangeListCallbackDelegate onMouseUpCallback;
        public ChangeListCallbackDelegate onReorderCallback;

        public ChangeDetailsCallbackDelegate onDetailsCallback;

        public CanChangeListCallbackDelegate onCanAddCallback;
        public CanChangeListCallbackDelegate onCanRemoveCallback;

        /// 
        /// All needed and provided layouts callbacks
        ///

        public ElementCallbackDelegate elementHeightCallback;


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
            throw new NotImplementedException();
        }


        private void DoListMiddle()
        { }

        private void DoListHeader()
        { }

        private void DoListFooter()
        { }


        #region Default interaction/draw methods

        public void AddNewDefaultElement()
        {
            Index = (List.arraySize += 1) - 1;
        }

        public void RemoveSelectedElement()
        {
            List.DeleteArrayElementAtIndex(Index);
            if (Index >= List.arraySize - 1)
            {
                Index = List.arraySize - 1;
            }
        }

        /// <summary>
        /// Default Footer behaviour.
        /// </summary>
        public void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.buttonArea, rect.y, Style.buttonArea, rect.height);
            //set rect properties from style
            var width = Style.buttonWidth;
            var height = Style.buttonHeight;
            var margin = Style.buttonMargin;
            var padding = Style.buttonPadding;
            //set proper rect for each buttons
            var addRect = new Rect(rect.xMin + margin, rect.y - padding, width, height);
            var removeRect = new Rect(rect.xMax - width - margin, rect.y - padding, width, height);

            EditorGUI.BeginDisabledGroup(List.hasMultipleDifferentValues);
            EditorGUI.BeginDisabledGroup(onCanAddCallback != null && !onCanAddCallback(this));
            if (GUI.Button(addRect, onAddDropdownCallback != null ? Style.iconToolbarDrop : Style.iconToolbarAdd, Style.footerButton))
            {
                if (onAddDropdownCallback != null)
                {
                    onAddDropdownCallback(addRect, this);
                }
                else if (onAddCallback != null)
                {
                    onAddCallback(this);
                }
                else
                {
                    AddNewDefaultElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            if (GUI.Button(removeRect, Style.iconToolbarRemove, Style.footerButton))
            {
                if (onRemoveCallback != null)
                {
                    onRemoveCallback(this);
                }
                else
                {
                    RemoveSelectedElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws Footer default background.
        /// </summary>
        public void DrawStandardFooterBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.footerBackground.Draw(new Rect(rect.xMax - Style.buttonArea, rect.y, Style.buttonArea, rect.height), false, false, false, false);
            }
        }

        /// <summary>
        /// Draws default Header's label.
        /// </summary>
        public void DrawStandardHeader(Rect rect)
        {
            rect.y += Style.spacing / 2;
            //display property name
            EditorGUI.LabelField(rect, List.displayName);
            rect.y += Style.spacing / 2;
            //adjust width and OX position for size property
            rect = new Rect(rect.xMax - Style.sizeArea, rect.y, Style.sizeArea, rect.height);

            //display array size property without indentation
            using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
            {
                var property = Size;
                EditorGUI.BeginDisabledGroup(HasFixedSize);
                EditorGUI.BeginProperty(rect, Style.arraySizeFieldContent, property);
                EditorGUI.BeginChangeCheck();
                //cache delayed size value using delayed int field
                var sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, property.intValue, Style.sizeLabel), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = sizeValue;
                }
                EditorGUI.EndProperty();
                EditorGUI.EndDisabledGroup();
            }
        }

        /// <summary>
        /// Draws default Header background.
        /// </summary>
        /// <param name="rect"></param>
        public void DrawStandardHeaderBackground(float height)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, height);
                Style.headerBackground.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draw default Element.
        /// </summary>
        public void DrawStandardElement(Rect rect, SerializedProperty element, bool selected, bool focused, bool draggable)
        {
            const string standardElementName = "Element";

            var displayName = element.displayName;
            if (ElementLabel != null)
            {
                displayName = element.displayName.Replace(standardElementName, ElementLabel);
            }

            var displayContent = new GUIContent(displayName);
            EditorGUIUtility.labelWidth -= Style.handleArea;
            EditorGUI.PropertyField(rect, element, displayContent, element.isExpanded);
            EditorGUIUtility.labelWidth += Style.handleArea;
        }

        /// <summary>
        /// Draws default DraggingHandle.
        /// </summary>
        public void DrawStandardElementDraggingHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (draggable)
            {
                rect.height = Style.handleHeight;
                rect.width = Style.handleWidth;
                rect.y += (Style.handleHeight + Style.spacing) / 2;
                rect.x += Style.handleWidth / 2;

                Style.draggingHandle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws default Element background.
        /// </summary>
        public void DrawStandardElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                //additional height for selection rect + shadow
                if (selected)
                {
                    rect.height += Style.spacing / 2;
                }
                Style.elementBackground.Draw(rect, false, selected, selected, focused);
            }
        }

        /// <summary>
        /// Draw default Element.
        /// </summary>
        public void DrawStandardNoneElement(Rect rect, bool draggable)
        {
            EditorGUI.LabelField(rect, Style.listIsEmptyContent);
        }

        #endregion


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


        /// <summary>
        /// Static representation of standard list style.
        /// Provides all needed <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        internal static class Style
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2;
#endif
            internal static readonly float padding = 6;
            internal static readonly float sizeArea = 19;

            internal static readonly float buttonArea = 60;
            internal static readonly float buttonWidth = 25;
            internal static readonly float buttonHeight = 13;
            internal static readonly float buttonMargin = 4;
            internal static readonly float buttonPadding = 3;

            internal static readonly float handleArea = 40;
            internal static readonly float handleWidth = 15;
            internal static readonly float handleHeight = 7;

            internal static readonly GUIContent iconToolbarAdd;
            internal static readonly GUIContent iconToolbarDrop;
            internal static readonly GUIContent iconToolbarRemove;
            internal static readonly GUIContent listIsEmptyContent;
            internal static readonly GUIContent arraySizeFieldContent;

            internal static readonly GUIStyle sizeLabel;
            internal static readonly GUIStyle footerButton;
            internal static readonly GUIStyle draggingHandle;
            internal static readonly GUIStyle headerBackground;
            internal static readonly GUIStyle middleBackground;
            internal static readonly GUIStyle footerBackground;
            internal static readonly GUIStyle elementBackground;

            static Style()
            {
                iconToolbarAdd = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
                iconToolbarDrop = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
                iconToolbarRemove = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                listIsEmptyContent = EditorGUIUtility.TrTextContent("List is Empty");
                arraySizeFieldContent = EditorGUIUtility.TrTextContent("", "List size");

                sizeLabel = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleRight
                };

                footerButton = new GUIStyle("RL FooterButton");
                draggingHandle = new GUIStyle("RL DragHandle");
                headerBackground = new GUIStyle("RL Header");
                middleBackground = new GUIStyle("RL Background");
                footerBackground = new GUIStyle("RL Footer");
                elementBackground = new GUIStyle("RL Element");
            }
        }
    }
}