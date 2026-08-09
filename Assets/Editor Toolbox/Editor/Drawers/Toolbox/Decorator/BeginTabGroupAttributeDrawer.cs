using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Toolbox.Editor.Drawers
{
    public sealed class BeginTabGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginTabGroupAttribute>
    {
        private class TabContext
        {
            public readonly int index;
            public readonly GUIContent content;
            public readonly float estimatedWidth;

            public TabContext(int index, GUIContent content, float estimatedWidth)
            {
                this.index = index;
                this.content = content;
                this.estimatedWidth = estimatedWidth;
            }
        }

        private class RowContext : IDisposable
        {
            public readonly List<TabContext> tabs;

            public RowContext()
            {
                tabs = new List<TabContext>();
            }

            public RowContext(List<TabContext> tabs)
            {
                this.tabs = tabs;
            }

            public void Append(TabContext tab)
            {
                tabs.Add(tab);
            }

            public bool Contains(int index)
            {
                for (var i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    if (tab.index == index)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Dispose()
            {
                tabs.Clear();
            }

            public int Count => tabs.Count;
        }

        protected override void OnGuiBeginSafe(BeginTabGroupAttribute attribute)
        {
            if (!TryGetDeclaringType(out var targetType))
            {
                return;
            }

            var groupId = attribute.GroupId;
            var tabs = TabsCacheManager.GetTabsForGroup(targetType, groupId);
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }

            var currentTabIndex = GetActiveTab(targetType, groupId, tabs);
            if (currentTabIndex == -1)
            {
                currentTabIndex = 0;
            }

            //TODO: create single method
            var rect = ToolboxLayoutHandler.BeginVertical(Style.allGroupStyle);
            rect = EditorGUI.IndentedRect(rect);
            if (Event.current.type == EventType.Repaint)
            {
                Style.backgroundStyle.Draw(rect, false, false, false, false);
            }

            var newIndex = DrawResponsiveTabs(currentTabIndex, tabs);
            if (newIndex != currentTabIndex)
            {
                var targetTab = tabs[newIndex];
                TabsCacheManager.TrySetIsTabActive(targetType, groupId, targetTab);
            }
        }

        private static int GetActiveTab(Type declaringType, string groupId, IReadOnlyList<string> tabs)
        {
            if (TabsCacheManager.TryGetActiveTabName(declaringType, groupId, out var activeTab))
            {
                for (var i = 0; i < tabs.Count; i++)
                {
                    var tab = tabs[i];
                    if (tab == activeTab)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        private static int DrawResponsiveTabs(int currentIndex, IReadOnlyList<string> labels)
        {
            var viewWidth = EditorGUIUtility.currentViewWidth - EditorGuiUtility.IndentSize - Style.viewPadding;

            var tabs = ListPool<TabContext>.Get();
            var rows = ListPool<RowContext>.Get();

            FetchTabs(labels, viewWidth, ref tabs);
            FetchRows(tabs, viewWidth, ref rows);
            RotateRowsToShowActiveTabLast(rows, currentIndex);
            var newIndex = DrawTabRows(rows, currentIndex);

            GUILayout.Space(Style.spaceAfterTabs);

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                row.Dispose();
                GenericPool<RowContext>.Release(row);
            }

            ListPool<TabContext>.Release(tabs);
            ListPool<RowContext>.Release(rows);

            return newIndex;
        }

        private static void FetchTabs(IReadOnlyList<string> labels, float viewWidth, ref List<TabContext> tabs)
        {
            for (var i = 0; i < labels.Count; i++)
            {
                var label = labels[i];

                //TODO: optimize it

                var content = new GUIContent(label);
                var size = Style.activeTabStyle.CalcSize(content);
                var width = size.x + Style.tabSpacing;

                width = Mathf.Max(width, Style.defaultTabWidth);
                width = Mathf.Min(width, Mathf.Max(Style.defaultTabWidth, viewWidth - Style.tabSpacing));

                var tab = new TabContext(i, content, width);
                tabs.Add(tab);
            }
        }

        private static void FetchRows(List<TabContext> tabs, float viewWidth, ref List<RowContext> rows)
        {
            var currentRow = GetNewRow();
            var currentRowWdith = 0.0f;

            for (var i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                var tabWidth = tab.estimatedWidth;

                if (currentRow.Count == 0)
                {
                    currentRow.Append(tab);
                    currentRowWdith = tabWidth;
                    continue;
                }

                if (currentRowWdith + tabWidth > viewWidth)
                {
                    rows.Add(currentRow);
                    currentRow = GetNewRow();
                    currentRow.Append(tab);
                    currentRowWdith = tabWidth;
                    continue;
                }

                currentRow.Append(tab);
                currentRowWdith += tabWidth;
            }

            if (currentRow.Count > 0)
            {
                rows.Add(currentRow);
            }
        }

        private static RowContext GetNewRow()
        {
            var row = GenericPool<RowContext>.Get();
            return row;
        }

        private static void RotateRowsToShowActiveTabLast(List<RowContext> rows, int currentIndex)
        {
            if (rows.Count <= 1)
            {
                return;
            }

            var activeRowIndex = FindRowContainingTab(rows, currentIndex, out var activeRow);
            if (activeRowIndex == rows.Count - 1)
            {
                return;
            }

            rows.RemoveAt(activeRowIndex);
            rows.Add(activeRow);
        }

        private static int FindRowContainingTab(List<RowContext> rows, int tabIndex, out RowContext activeRow)
        {
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row.Contains(tabIndex))
                {
                    activeRow = row;
                    return i;
                }
            }

            activeRow = default;
            return 0;
        }

        private static int DrawTabRows(List<RowContext> rows, int currentIndex)
        {
            var style = Style.rowGroupStyle;
            style.margin.left = (int)EditorGuiUtility.IndentSize;

            var newIndex = currentIndex;
            using (new EditorGUILayout.VerticalScope(style))
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    GUILayout.BeginHorizontal();
                    newIndex = DrawRowTabs(row, currentIndex, newIndex);
                    GUILayout.EndHorizontal();

                    if (i < rows.Count - 1)
                    {
                        GUILayout.Space(Style.rowSpacing);
                    }
                }
            }

            return newIndex;
        }

        private static int DrawRowTabs(RowContext row, int currentIndex, int newIndex)
        {
            var tabs = row.tabs;
            for (var i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                var tabIndex = tab.index;
                var tabLabel = tab.content;
                var isActive = tabIndex == currentIndex;

                var previousBackground = GUI.backgroundColor;

                GUI.backgroundColor = isActive
                    ? Style.activeTabBackgroundColor
                    : Style.defaultTabBackgroundColor;

                var style = isActive
                    ? Style.activeTabStyle
                    : Style.defaultTabStyle;
                var pressed = GUILayout.Toggle(isActive, tabLabel, style);

                GUI.backgroundColor = previousBackground;

                if (pressed && !isActive)
                {
                    newIndex = tabIndex;
                }

                if (i < row.Count - 1)
                {
                    GUILayout.Space(Style.tabSpacing);
                }
            }

            return newIndex;
        }

        private static class Style
        {
            internal const float tabSpacing = 2.5f;
            internal const float rowSpacing = 2.0f;
            internal const float defaultTabWidth = 40.0f;
            internal const float defaultTabHeight = 20.0f;
            internal const float viewPadding = 32.0f;
            internal const float spaceAfterTabs = 4.0f;

            internal static readonly Color defaultBackgroundMultiplier = new(0.9f, 0.9f, 0.9f, 0.5f);
            internal static readonly Color activeTabBackgroundColor;
            internal static readonly Color defaultTabBackgroundColor;

            internal static readonly GUIStyle activeTabStyle;
            internal static readonly GUIStyle defaultTabStyle;
            internal static readonly GUIStyle backgroundStyle;
            internal static readonly GUIStyle allGroupStyle;
            internal static readonly GUIStyle rowGroupStyle;

            static Style()
            {
                activeTabBackgroundColor = EditorGuiUtility.BasicBackgroundColor;
                defaultTabBackgroundColor = EditorGuiUtility.BasicBackgroundColor * defaultBackgroundMultiplier;

                defaultTabStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fixedHeight = defaultTabHeight,
                    padding = new RectOffset(10, 10, 4, 4),
                };

                activeTabStyle = new GUIStyle(defaultTabStyle)
                {
                    fontStyle = FontStyle.Bold
                };
                activeTabStyle.normal.background = Texture2D.whiteTexture;

                backgroundStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    border = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(1, 1, 1, 1),
                };

                allGroupStyle = new GUIStyle()
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    border = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(1, 1, 1, 1)
                };

                rowGroupStyle = new GUIStyle()
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    border = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(1, 0, 0, 0)
                };
            }
        }
    }
}