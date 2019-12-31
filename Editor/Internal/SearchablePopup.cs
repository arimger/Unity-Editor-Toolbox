//Custom reimplementation of an idea orginally provided here - https://github.com/roboryantron/UnityEditorJunkie/blob/master/Assets/SearchableEnum/Code/Editor/SearchablePopup.cs, 2019

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Searchable popup content that allows user to filter items using provided string value.
    /// </summary>
    public class SearchablePopup : PopupWindowContent
    {
        public static void Show(Rect activatorRect, int current, string[] options, Action<int> onSelect)
        { 
            PopupWindow.Show(activatorRect, new SearchablePopup(options, current, onSelect));
        }


        private readonly Action<int> onSelect;

        private readonly SearchList searchList;
        private readonly SearchField searchField;

        private readonly int startIndex;


        private int scrollIndex = -1;
        private int hoverIndex = -1;

        private Vector2 scroll;

        private Rect toolbarRect;
        private Rect contentRect;


        private SearchablePopup(string[] options, int startIndex, Action<int> onSelect)
        {
            searchList = new SearchList(options);
            searchField = new SearchField();

            this.onSelect = onSelect;
            this.startIndex = startIndex;
        }


        private void MakeSelection(int index)
        {
            onSelect(index);
            editorWindow.Close();
        }

        private void HandleKeyboard()
        {
            var currentEvent = Event.current;

            if (currentEvent.type == EventType.KeyDown)
            {
                if (currentEvent.keyCode == KeyCode.DownArrow)
                {
                    GUI.FocusControl(null);
                    hoverIndex = Mathf.Min(searchList.ItemsCount - 1, hoverIndex + 1);
                    scrollIndex = hoverIndex;
                    currentEvent.Use();
                }

                if (currentEvent.keyCode == KeyCode.UpArrow)
                {
                    GUI.FocusControl(null);
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    scrollIndex = hoverIndex;
                    currentEvent.Use();
                }

                if (currentEvent.keyCode == KeyCode.Return)
                {
                    GUI.FocusControl(null);
                    if (hoverIndex >= 0 && hoverIndex < searchList.ItemsCount)
                    {
                        MakeSelection(searchList.GetItemAt(hoverIndex).index);
                    }
                    currentEvent.Use();
                }

                if (currentEvent.keyCode == KeyCode.Escape)
                {
                    GUI.FocusControl(null);
                    editorWindow.Close();
                }
            }
        }

        private void DrawToolbar(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Style.toolbarStyle.Draw(rect, false, false, false, false);
            }

            rect.xMin += Style.padding;
            rect.xMax -= Style.padding;
            rect.yMin += Style.spacing;
            rect.yMax -= Style.spacing;

            searchList.Search(searchField.OnGUI(rect, searchList.Filter, Style.searchBoxStyle, Style.enabledCancelButtonStyle, Style.disabledCancelButtonStyle));
        }

        private void DrawContent(Rect rect)
        {
            var currentEvent = Event.current;

            var contentRect = new Rect(0, 0, rect.width - Style.scrollbarStyle.fixedWidth, searchList.ItemsCount * Style.height);
            var optionRect = new Rect(0, 0, rect.width, Style.height);

            scroll = GUI.BeginScrollView(rect, scroll, contentRect);

            for (var i = 0; i < searchList.ItemsCount; i++)
            {
                if (currentEvent.type == EventType.Repaint && scrollIndex == i)
                {
                    GUI.ScrollTo(optionRect);
                    scrollIndex = -1;
                }
                if (optionRect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.type == EventType.MouseMove || currentEvent.type == EventType.ScrollWheel)
                    {
                        hoverIndex = i;
                    }
                    if (currentEvent.type == EventType.MouseDown)
                    {
                        MakeSelection(searchList.GetItemAt(i).index);
                    }
                }

                if (hoverIndex == i)
                {
                    GUI.Box(optionRect, GUIContent.none, Style.selectionStyle);
                }

                optionRect.xMin += Style.indent;
                GUI.Label(optionRect, searchList.GetItemAt(i).label);
                optionRect.xMin -= Style.indent;
                optionRect.y = optionRect.yMax;
            }

            GUI.EndScrollView();
        }


        public override void OnOpen()
        {
            EditorApplication.update += editorWindow.Repaint;
        }

        public override void OnClose()
        {
            EditorApplication.update -= editorWindow.Repaint;
        }

        public override void OnGUI(Rect rect)
        {
            //set toolbar rect based on built-in toolbar style
            toolbarRect = new Rect(0, 0, rect.width, Style.toolbarStyle.fixedHeight);
            //set content rect adjusted to toolbar
            contentRect = Rect.MinMaxRect(0, toolbarRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawToolbar(toolbarRect);
            DrawContent(contentRect);
            GUI.enabled = false;
        }


        /// <summary>
        /// Helper class used in options filtering.
        /// </summary>
        private class SearchList
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


            private readonly string[] options;

            private readonly List<Item> items;


            public SearchList(string[] options)
            {
                this.options = options;
                items = new List<Item>();
                Search("");
            }


            public bool Search(string filter)
            {
                if (Filter == filter)
                {
                    return false;
                }

                items.Clear();

                for (int i = 0; i < options.Length; i++)
                {
                    if (string.IsNullOrEmpty(filter) || options[i].ToLower().Contains(filter.ToLower()))
                    {
                        var item = new Item(i, options[i]);

                        if (string.Equals(options[i], filter, StringComparison.CurrentCultureIgnoreCase))
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


            public int OptionsCount => options.Length;

            public int ItemsCount => items.Count;

            public string Filter { get; private set; }
        }


        /// <summary>
        /// Custom styling.
        /// </summary>
        internal static class Style
        {
            internal static readonly float padding = 6.0f;
            internal static readonly float spacing = 2.0f;
            internal static readonly float indent = 8.0f;
            internal static readonly float height = EditorGUIUtility.singleLineHeight;

            internal static GUIStyle toolbarStyle;
            internal static GUIStyle scrollbarStyle;
            internal static GUIStyle selectionStyle;
            internal static GUIStyle searchBoxStyle;
            internal static GUIStyle enabledCancelButtonStyle;
            internal static GUIStyle disabledCancelButtonStyle;

            static Style()
            {
                toolbarStyle = new GUIStyle(EditorStyles.toolbar);
                scrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                selectionStyle = new GUIStyle("SelectionRect");
                searchBoxStyle = new GUIStyle("ToolbarSeachTextField");
                enabledCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButton");
                disabledCancelButtonStyle = new GUIStyle("ToolbarSeachCancelButtonEmpty");
            }
        }
    }
}