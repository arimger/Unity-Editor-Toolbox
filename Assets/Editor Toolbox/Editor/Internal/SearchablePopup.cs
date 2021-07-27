//Custom reimplementation of an idea originally provided here - https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchablePopup.cs, 2019

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Searchable popup content that allows user to filter items using a provided string value.
    /// </summary>
    public class SearchablePopup : PopupWindowContent
    {
        /// <summary>
        /// Creates searchable popup using given properties.
        /// </summary>
        public static void Show(Rect activatorRect, int current, string[] options, Action<int> onSelect)
        {
            PopupWindow.Show(activatorRect, new SearchablePopup(current, options, onSelect));
        }


        private readonly Action<int> onSelect;

        private readonly SearchArray searchArray;
        private readonly SearchField searchField;

        private int optionIndex = -1;
        private int scrollIndex = -1;

        private Vector2 scroll;

        private Rect toolbarRect;
        private Rect contentRect;


        /// <summary>
        /// Constructor should be called only internally by the <see cref="Show(Rect, int, string[], Action{int})"/> method.
        /// </summary>
        private SearchablePopup(int startIndex, string[] options, Action<int> onSelect)
        {
            searchArray = new SearchArray(options);
            searchField = new SearchField();

            optionIndex = startIndex;
            scrollIndex = startIndex;

            this.onSelect = onSelect;
            this.onSelect += (i) =>
            {
                editorWindow.Close();
            };
        }


        private void SelectItem(int index)
        {
            onSelect(index);
        }

        private void HandleInput()
        {
            var currentEvent = Event.current;
            if (currentEvent.type != EventType.KeyDown)
            {
                return;
            }

            if (currentEvent.keyCode == KeyCode.DownArrow)
            {
                GUI.FocusControl(null);
                optionIndex = Mathf.Min(searchArray.ItemsCount - 1, optionIndex + 1);
                scrollIndex = optionIndex;
                currentEvent.Use();
            }

            if (currentEvent.keyCode == KeyCode.UpArrow)
            {
                GUI.FocusControl(null);
                optionIndex = Mathf.Max(0, optionIndex - 1);
                scrollIndex = optionIndex;
                currentEvent.Use();
            }

            if (currentEvent.keyCode == KeyCode.Return)
            {
                GUI.FocusControl(null);
                if (optionIndex >= 0 && optionIndex < searchArray.ItemsCount)
                {
                    SelectItem(searchArray.GetItemAt(optionIndex).index);
                }

                currentEvent.Use();
            }

            if (currentEvent.keyCode == KeyCode.Escape)
            {
                GUI.FocusControl(null);
                editorWindow.Close();
            }
        }

        private void DrawToolbar(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.toolbarStyle.Draw(rect, false, false, false, false);
            }

            rect.xMin += Style.padding;
            //NOTE: in Unity 2019.x "cancel" button is outside the search field
#if !UNITY_2019_3_OR_NEWER || UNITY_2020_1_OR_NEWER
            rect.xMax -= Style.padding;
#endif
            rect.yMin += Style.spacing;
            rect.yMax -= Style.spacing;

            //draw toolbar and try to search for valid text
            var filter = searchArray.Filter;
            var result = searchField.OnGUI(rect, filter, Style.searchBoxStyle,
                Style.showCancelButtonStyle, Style.hideCancelButtonStyle);
            searchArray.Search(result);
        }

        private void DrawContent(Rect rect)
        {
            var currentEvent = Event.current;

            var itemsCount = searchArray.ItemsCount;
            //prepare base rect for the whole content window
            var contentRect = new Rect(0, 0, rect.width - Style.scrollbarStyle.fixedWidth, itemsCount * Style.height);
            var elementRect = new Rect(0, 0, rect.width, Style.height);

            scroll = GUI.BeginScrollView(rect, scroll, contentRect);
            //iterate over all searched and available items
            for (var i = 0; i < itemsCount; i++)
            {
                if (currentEvent.type == EventType.Repaint && scrollIndex == i)
                {
                    GUI.ScrollTo(elementRect);
                    scrollIndex = -1;
                }

                if (elementRect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.type == EventType.MouseMove || currentEvent.type == EventType.ScrollWheel)
                    {
                        optionIndex = i;
                    }

                    if (currentEvent.type == EventType.MouseDown)
                    {
                        SelectItem(searchArray.GetItemAt(i).index);
                    }
                }

                if (optionIndex == i)
                {
                    GUI.Box(elementRect, GUIContent.none, Style.selectionStyle);
                }

                //draw proper label for the associated item
                elementRect.xMin += Style.indent;
                GUI.Label(elementRect, GetElementContent(elementRect, i));
                elementRect.xMin -= Style.indent;
                elementRect.y = elementRect.yMax;
            }

            GUI.EndScrollView();
        }

        private GUIContent GetElementContent(Rect rect, int index)
        {
            var itemLabel = searchArray.GetItemAt(index).label;
            var content = new GUIContent(itemLabel);
            var labelSize = EditorStyles.label.CalcSize(content);
            if (labelSize.x > rect.width)
            {
                content.tooltip = itemLabel;
            }

            return content;
        }


        /// <summary>
        /// Called each time new <see cref="SearchablePopup"/> is created.
        /// </summary>
        public override void OnOpen()
        {
            EditorApplication.update += editorWindow.Repaint;
        }

        /// <summary>
        /// Called each time new <see cref="SearchablePopup"/> is closed.
        /// </summary>
        public override void OnClose()
        {
            EditorApplication.update -= editorWindow.Repaint;
        }

        /// <summary>
        /// Called each time <see cref="SearchablePopup"/> has to be drawed.
        /// </summary>
        public override void OnGUI(Rect rect)
        {
            //set toolbar rect using the built-in toolbar height
            toolbarRect = new Rect(0, 0, rect.width, Style.toolbarStyle.fixedHeight);
            //set content rect adjusted to the toolbar container
            contentRect = Rect.MinMaxRect(0, toolbarRect.yMax, rect.xMax, rect.yMax);

            HandleInput();
            DrawToolbar(toolbarRect);
            DrawContent(contentRect);
            //additionally disable all GUI controls
            GUI.enabled = false;
        }


        private class SearchArray
        {
            public struct Item
            {
                public int index;
                public string label;

                public Item(int index, string label)
                {
                    this.index = index;
                    this.label = label;
                }
            }


            private readonly List<Item> items;
            private readonly string[] options;


            public SearchArray(string[] options)
            {
                this.options = options;
                items = new List<Item>();
                Search(string.Empty);
            }


            public bool Search(string filter)
            {
                if (Filter == filter)
                {
                    return false;
                }

                items.Clear();
                var simplifiedFilter = filter.ToLower();
                for (var i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (string.IsNullOrEmpty(filter) || option.ToLower().Contains(simplifiedFilter))
                    {
                        var item = new Item(i, option);
                        if (string.Equals(option, filter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            items.Insert(0, item);
                        }
                        else
                        {
                            items.Add(item);
                        }
                    }
                }

                Filter = filter;
                return true;
            }

            public Item GetItemAt(int index)
            {
                return items[index];
            }


            public int ItemsCount => items.Count;

            public string Filter { get; private set; }
        }

        private static class Style
        {
            internal static readonly float indent = 8.0f;
            internal static readonly float height = EditorGUIUtility.singleLineHeight;
            internal static readonly float padding = 6.0f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static GUIStyle toolbarStyle;
            internal static GUIStyle scrollbarStyle;
            internal static GUIStyle selectionStyle;
            internal static GUIStyle searchBoxStyle;
            internal static GUIStyle showCancelButtonStyle;
            internal static GUIStyle hideCancelButtonStyle;

            static Style()
            {
                toolbarStyle = new GUIStyle(EditorStyles.toolbar);
                scrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                selectionStyle = new GUIStyle("SelectionRect");
                searchBoxStyle = new GUIStyle("ToolbarSeachTextField");
                showCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButton");
                hideCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButtonEmpty");
            }
        }
    }
}