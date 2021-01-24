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

        public delegate bool CanChangeListCallbackDelegate(ReorderableListBase list);
        public delegate void ChangeDetailsCallbackDelegate(ReorderableListBase list, int oldIndex, int newIndex);
        public delegate void IsChangedListCallbackDelegate(ReorderableListBase list);

        public delegate void DrawRelatedRectCallbackDelegate(Rect rect, ReorderableListBase list);


        public DrawRectCallbackDelegate drawHeaderCallback;
        public DrawRectCallbackDelegate drawVoidedCallback;
        public DrawRectCallbackDelegate drawFooterCallback;
        public DrawRectCallbackDelegate drawHandleCallback;

        public DrawRectCallbackDelegate drawHeaderBackgroundCallback;
        public DrawRectCallbackDelegate drawFooterBackgroundCallback;
        public DrawRectCallbackDelegate drawMiddleBackgroundCallback;

        public DrawRelatedRectCallbackDelegate onAppendDropdownCallback;

        public IsChangedListCallbackDelegate onAppendCallback;
        public IsChangedListCallbackDelegate onRemoveCallback;
        public IsChangedListCallbackDelegate onSelectCallback;

        public IsChangedListCallbackDelegate onChangedCallback;
        public IsChangedListCallbackDelegate onMouseUpCallback;
        public IsChangedListCallbackDelegate onReorderCallback;

        public ChangeDetailsCallbackDelegate onDetailsCallback;

        public CanChangeListCallbackDelegate onCanAppendCallback;
        public CanChangeListCallbackDelegate onCanRemoveCallback;


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


        protected virtual void DoListHeader()
        {
            if (!HasHeader)
            {
                return;
            }

            var rect = GUILayoutUtility.GetRect(0, HeaderHeight, GUILayout.ExpandWidth(true));
            DoListHeader(rect);
        }

        protected virtual void DoListHeader(Rect headerRect)
        {
            if (!HasHeader)
            {
                return;
            }

            //draw the background on repaint
            if (Event.current.type == EventType.Repaint)
            {
                if (drawHeaderBackgroundCallback != null)
                {
                    drawHeaderBackgroundCallback(headerRect);
                }
                else
                {
                    DrawStandardHeaderBackground(headerRect);
                }
            }

            //apply the padding to get the internal rect
            headerRect.xMin += Style.padding;
            headerRect.xMax -= Style.padding;
            headerRect.yMin -= Style.spacing / 2;
            headerRect.yMax += Style.spacing / 2;

            //perform the default or overridden callback
            if (drawHeaderCallback != null)
            {
                drawHeaderCallback(headerRect);
            }
            else
            {
                DrawStandardHeader(headerRect);
            }
        }

        protected virtual void DoListFooter()
        {
            if (FixedSize)
            {
                return;
            }

            var rect = GUILayoutUtility.GetRect(0, FooterHeight, GUILayout.ExpandWidth(true));
            DoListFooter(rect);
        }

        protected virtual void DoListFooter(Rect footerRect)
        {
            //ignore footer if list has fixed size
            if (FixedSize)
            {
                return;
            }

            //draw the background on repaint event
            if (Event.current.type == EventType.Repaint)
            {
                if (drawFooterBackgroundCallback != null)
                {
                    drawFooterBackgroundCallback(footerRect);
                }
                else
                {
                    DrawStandardFooterBackground(footerRect);
                }
            }

            //perform callback or the default footer
            if (drawFooterCallback != null)
            {
                drawFooterCallback(footerRect);
            }
            else
            {
                DrawStandardFooter(footerRect);
            }
        }


        protected abstract void DoListMiddle();

        protected abstract void DoListMiddle(Rect middleRect);


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


        public virtual void DoList()
        {
            //NOTE: indentation will break some controls
            //make sure there is no indent while drawing
            using (new ZeroIndentScope())
            {
                DoListHeader();
                DoListMiddle();
                DoListFooter();
            }
        }


        #region Methods: Default interaction/draw calls

        /// <summary>
        /// Draws the default Footer.
        /// </summary>
        public void DrawStandardFooter(Rect rect)
        {
            //set button area rect
            rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);

            //set rect properties from style
            var buttonWidth = Style.footerButtonWidth;
            var buttonHeight = Style.footerButtonHeight;
            var margin = Style.footerMargin;
            var padding = Style.footerPadding;

            //set proper rect for each buttons
            var appendButtonRect = new Rect(rect.xMin + margin, rect.y - padding, buttonWidth, buttonHeight);

            EditorGUI.BeginDisabledGroup(List.hasMultipleDifferentValues);
            EditorGUI.BeginDisabledGroup(onCanAppendCallback != null && !onCanAppendCallback(this));
            if (GUI.Button(appendButtonRect, onAppendDropdownCallback != null
                ? Style.iconToolbarDropContent
                : Style.iconToolbarAddContent, Style.footerButtonStyle))
            {
                if (onAppendDropdownCallback != null)
                {
                    onAppendDropdownCallback(appendButtonRect, this);
                }
                else if (onAppendCallback != null)
                {
                    onAppendCallback(this);
                }
                else
                {
                    AppendElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();

            var removeButtonRect = new Rect(rect.xMax - buttonWidth - margin, rect.y - padding, buttonWidth, buttonHeight);

            EditorGUI.BeginDisabledGroup((onCanRemoveCallback != null && !onCanRemoveCallback(this) || Index < 0 || Index >= Count));
            if (GUI.Button(removeButtonRect, Style.iconToolbarRemoveContent, Style.footerButtonStyle))
            {
                if (onRemoveCallback != null)
                {
                    onRemoveCallback(this);
                }
                else
                {
                    RemoveElement();
                }

                onChangedCallback?.Invoke(this);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws the default Footer background.
        /// </summary>
        public void DrawStandardFooterBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                rect = new Rect(rect.xMax - Style.footerWidth, rect.y, Style.footerWidth, rect.height);
                Style.footerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Header.
        /// </summary>
        public void DrawStandardHeader(Rect rect)
        {
            var label = EditorGUI.BeginProperty(rect, new GUIContent(List.displayName), List);

            var diff = rect.height - Style.sizePropertyStyle.fixedHeight;
            var oldY = rect.y;

#if !UNITY_2019_3_OR_NEWER
            //adjust OY position to middle of the conent
            rect.y += diff / 2;
#endif
            //display the property label using the preprocessed name by BeginProperty method
            EditorGUI.LabelField(rect, label);

            //adjust OY position to the middle of the element row
            rect.y = oldY;
            rect.yMin += diff / 2;
            rect.yMax -= diff / 2;
            //adjust OX position and width for the size property
            rect.xMin = rect.xMax - Style.sizeAreaWidth;

            using (new EditorGUI.DisabledScope(FixedSize))
            {
                var property = Size;

                EditorGUI.BeginProperty(rect, Style.sizePropertyContent, property);
                EditorGUI.BeginChangeCheck();
                //cache a delayed size value using the delayed int field
                var sizeValue = Mathf.Max(EditorGUI.DelayedIntField(rect, property.intValue, Style.sizePropertyStyle), 0);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = sizeValue;
                }
                EditorGUI.EndProperty();
            }
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws the default Header background.
        /// </summary>
        /// <param name="rect"></param>
        public void DrawStandardHeaderBackground(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.headerBackgroundStyle.Draw(rect, false, false, false, false);
            }
        }

        /// <summary>
        /// Draws the default Element field.
        /// </summary>
        public void DrawStandardElement(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            var element = List.GetArrayElementAtIndex(index);
            var label = HasLabels
                ? new GUIContent(GetElementDisplayName(element, index))
                : new GUIContent();
            EditorGUI.PropertyField(rect, element, label, element.isExpanded);
        }

        /// <summary>
        /// Draws the default dragging Handle.
        /// </summary>
        public void DrawStandardElementHandle(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                //keep the dragging handle in the 1 row
                rect.yMax = rect.yMin + EditorGUIUtility.singleLineHeight;

                //prepare rect for the handle texture draw
                var xDiff = rect.width - Style.handleWidth;
                rect.xMin += xDiff / 2;
                rect.xMax -= xDiff / 2;

                var yDiff = rect.height - Style.handleHeight;
                rect.yMin += yDiff / 2;
                rect.yMax -= yDiff / 2;
#if UNITY_2019_3_OR_NEWER
                rect.y += Style.spacing;
#endif
                //disable (if needed) and draw the handle
                using (new EditorGUI.DisabledScope(!draggable))
                {
                    Style.dragHandleButtonStyle.Draw(rect, false, false, false, false);
                }
            }
        }

        /// <summary>                                       
        /// Draws the default Element background.
        /// </summary>
        public void DrawStandardElementBackground(Rect rect, int index, bool selected, bool focused, bool draggable)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (selected)
                {
                    //additional height for selection rect + shadow
                    //NOTE: shadow appears only before Unity 2019.3
#if UNITY_2019_3_OR_NEWER
                    var padding = Style.spacing;
#else
                    var padding = Style.spacing + Style.spacing / 2;
#endif
                    rect.yMax += padding / 2;
                    rect.yMin -= padding / 2;
                }

                Style.elementBackgroundStyle.Draw(rect, false, selected, selected, focused);
            }
        }

        #endregion


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


        /// <summary>
        /// Static representation of the standard list style.
        /// Provides all needed <see cref="GUIStyle"/>s, paddings, widths, heights, etc.
        /// </summary>
        internal static class Style
        {
#if UNITY_2018_3_OR_NEWER
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
#else
            internal static readonly float spacing = 2.0f;
#endif
            internal static readonly float padding = 6.0f;

            internal static readonly float lineHeight = EditorGUIUtility.singleLineHeight;

            internal static readonly float footerWidth = 60.0f;
            internal static readonly float footerButtonWidth = 25.0f;
            internal static readonly float footerButtonHeight = 13.0f;
            internal static readonly float footerMargin = 4.0f;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float footerPadding = 0.0f;
#else
            internal static readonly float footerPadding = 3.0f;
#endif
            internal static readonly float handleWidth = 15.0f;
            internal static readonly float handleHeight = 7.0f;
            internal static readonly float dragAreaWidth = 40.0f;
            internal static readonly float sizeAreaWidth = 19.0f;

            internal static readonly GUIContent sizePropertyContent;
            internal static readonly GUIContent iconToolbarAddContent;
            internal static readonly GUIContent iconToolbarDropContent;
            internal static readonly GUIContent iconToolbarRemoveContent;
            internal static readonly GUIContent emptyOrInvalidListContent;

            internal static readonly GUIStyle sizePropertyStyle;
            internal static readonly GUIStyle footerButtonStyle;
            internal static readonly GUIStyle dragHandleButtonStyle;
            internal static readonly GUIStyle headerBackgroundStyle;
            internal static readonly GUIStyle middleBackgroundStyle;
            internal static readonly GUIStyle footerBackgroundStyle;
            internal static readonly GUIStyle elementBackgroundStyle;

            static Style()
            {
                sizePropertyContent = EditorGUIUtility.TrTextContent("", "List size");
                iconToolbarAddContent = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
                iconToolbarDropContent = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
                iconToolbarRemoveContent = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                emptyOrInvalidListContent = EditorGUIUtility.TrTextContent("List is Empty");

                sizePropertyStyle = new GUIStyle(EditorStyles.miniTextField)
                {
                    alignment = TextAnchor.MiddleRight,
                    //NOTE: in newer releases, the font size has to be adjusted
#if UNITY_2019_3_OR_NEWER
                    fixedHeight = 14.0f,
                    fontSize = 10
#endif
                };

                footerButtonStyle = new GUIStyle("RL FooterButton");
                dragHandleButtonStyle = new GUIStyle("RL DragHandle");
                headerBackgroundStyle = new GUIStyle("RL Header");
                middleBackgroundStyle = new GUIStyle("RL Background");
                footerBackgroundStyle = new GUIStyle("RL Footer");
                elementBackgroundStyle = new GUIStyle("RL Element");
            }
        }
    }
}