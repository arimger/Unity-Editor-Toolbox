using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Toolbox.Editor.Drawers
{
    public sealed class BeginTabGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginTabGroupAttribute>
    {
        #region CONSTANTS
        private const float ViewWidthPadding = 32f;
        private const float TabSpacing = 8f;
        private const float MinTabWidth = 40f;
        private const float TabHeight = 22f;
        private const float RowSpacing = 2.0f;
        private const float ToggleSpacing = 2.5f;

        //TODO: just make as separate color
        private static readonly Color InactiveBgMultiplier = new(0.6f, 0.6f, 0.6f, 0.6f);

        //TODO: move to utility
        private static Color ActiveBgColor => EditorGUIUtility.isProSkin
            ? new Color(0.25f, 0.25f, 0.25f)
            : new Color(0.81f, 0.81f, 0.81f);

        #endregion

        //TODO: create Style class
        #region STYLE

        private static GUIStyle _baseTabStyle;
        private static GUIStyle _activeTabStyle;
        private static GUIStyle _headerStyle;

        private static GUIStyle BaseTabStyle
        {
            get
            {
                _baseTabStyle ??= new GUIStyle(EditorStyles.toolbarButton)
                {
                    fixedHeight = TabHeight,
                    padding = new RectOffset(10, 10, 4, 4),
                };
                return _baseTabStyle;
            }
        }

        private static GUIStyle ActiveTabStyle
        {
            get
            {
                _activeTabStyle ??= new GUIStyle(BaseTabStyle) { fontStyle = FontStyle.Bold };
                _activeTabStyle.normal.background = new Texture2D(1, 1);
                _activeTabStyle.normal.background.SetPixel(0, 0, Color.white);
                _activeTabStyle.normal.background.Apply();
                return _activeTabStyle;
            }
        }

        private static GUIStyle HeaderStyle
        {
            get
            {
                _headerStyle ??= new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(0, 0, 2, 2),
                };
                return _headerStyle;
            }
        }

        #endregion

        protected override void OnGuiBeginSafe(BeginTabGroupAttribute attribute)
        {
            if (!TryGetDeclaringType(out var targetType))
            {
                return;
            }

            var groupId = attribute.GroupId;
            var tabs = TabDiscovery.GetTabsForGroup(targetType, groupId);
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }

            InitializeDefaultTab(groupId, tabs);

            var currentTabIndex = GetActiveTab(groupId, tabs);
            if (currentTabIndex == -1)
            {
                currentTabIndex = 0;
            }

            //TODO: temp, make it static
            var style = new GUIStyle(EditorStyles.helpBox);
            style.margin = new RectOffset(0, 0, 0, 0);
            style.contentOffset = Vector2.zero;
            style.border = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(1, 1, 1, 1);

            ToolboxLayoutHandler.BeginVertical(style);

            var newIndex = DrawResponsiveTabs(currentTabIndex, tabs);
            if (newIndex != currentTabIndex)
            {
                TabState.Set(groupId, tabs[newIndex]);
            }
        }

        private static void InitializeDefaultTab(string groupId, IReadOnlyList<string> tabs)
        {
            if (TabState.Has(groupId))
            {
                return;
            }

            if (tabs.Count > 0)
            {
                TabState.Set(groupId, tabs[0]);
            }
        }

        private static int GetActiveTab(string groupId, IReadOnlyList<string> tabs)
        {
            if (TabState.TryGet(groupId, out var activeTab))
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

        private static int DrawResponsiveTabs(int currentIndex, IReadOnlyList<string> labels)
        {
            var viewWidth = EditorGUIUtility.currentViewWidth - ViewWidthPadding;

            var tabs = ListPool<TabContext>.Get();
            var rows = ListPool<RowContext>.Get();

            FetchTabs(labels, viewWidth, ref tabs);
            FetchRows(tabs, viewWidth, ref rows);
            RotateRowsToShowActiveTabLast(rows, currentIndex);
            int newIndex = DrawTabRows(rows, currentIndex);

            //TODO: style
            GUILayout.Space(4.0f);

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
                var size = ActiveTabStyle.CalcSize(content);
                var width = size.x + TabSpacing;

                width = Mathf.Max(width, MinTabWidth);
                width = Mathf.Min(width, Mathf.Max(MinTabWidth, viewWidth - TabSpacing));

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
            var newIndex = currentIndex;
            using (new EditorGUILayout.VerticalScope())
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    GUILayout.BeginHorizontal();
                    newIndex = DrawRowButtons(row, currentIndex, newIndex);
                    GUILayout.EndHorizontal();

                    if (i < rows.Count - 1)
                    {
                        GUILayout.Space(RowSpacing);
                    }
                }
            }

            return newIndex;
        }

        private static int DrawRowButtons(RowContext row, int currentIndex, int newIndex)
        {
            var tabs = row.tabs;
            for (var i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                var tabIndex = tab.index;
                var tabLabel = tab.content;
                var isActive = tabIndex == currentIndex;

                var prevBg = GUI.backgroundColor;

                GUI.backgroundColor = isActive
                    ? ActiveBgColor
                    : GUI.backgroundColor * InactiveBgMultiplier;
                var style = isActive ? ActiveTabStyle : BaseTabStyle;
                bool pressed = GUILayout.Toggle(isActive, tabLabel, style);
                GUI.backgroundColor = prevBg;

                if (pressed && !isActive)
                {
                    newIndex = tabIndex;
                }

                if (i < row.Count - 1)
                {
                    GUILayout.Space(ToggleSpacing);
                }
            }

            return newIndex;
        }
    }

    public sealed class TabAttributeDrawer : ToolboxConditionDrawer<TabAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, TabAttribute attribute)
        {
            //TODO: better way to get the unique ID
            var targetType = property.GetDeclaringObject().GetType();
            var groupId = TabDiscovery.GetGroupForField(targetType, property.name);

            if (string.IsNullOrEmpty(groupId))
            {
                return PropertyCondition.Valid;
            }

            return TabState.IsActive(groupId, attribute.Tab)
                ? PropertyCondition.Valid
                : PropertyCondition.NonValid;
        }
    }

    public sealed class EndTabGroupAttributeDrawer : ToolboxDecoratorDrawer<EndTabGroupAttribute>
    {
        protected override void OnGuiCloseSafe(EndTabGroupAttribute attribute)
        {
            ToolboxLayoutHandler.CloseVertical();
        }
    }

    internal static class TabState
    {
        private static readonly Dictionary<string, string> ActiveTabs = new();

        public static void Set(string groupId, string tab)
        {
            ActiveTabs[groupId] = tab;
        }

        public static bool TryGet(string groupId, out string tab)
        {
            return ActiveTabs.TryGetValue(groupId, out tab);
        }

        public static bool IsActive(string groupId, string tab)
        {
            return ActiveTabs.TryGetValue(groupId, out var active) && active == tab;
        }

        public static bool Has(string groupId)
        {
            return ActiveTabs.ContainsKey(groupId);
        }
    }

    internal static class TabDiscovery
    {
        private struct GroupData
        {
            public Dictionary<string, List<string>> GroupToTabs;
            public Dictionary<string, string> FieldToGroup;
        }

        private static readonly Dictionary<Type, GroupData> TypeCache = new();

        public static IReadOnlyList<string> GetTabsForGroup(Type type, string groupId)
        {
            EnsureCached(type);
            return TypeCache[type].GroupToTabs.TryGetValue(groupId, out var tabs) ? tabs : null;
        }

        public static string GetGroupForField(Type type, string fieldName)
        {
            EnsureCached(type);
            return TypeCache[type].FieldToGroup.TryGetValue(fieldName, out var group)
                ? group
                : null;
        }

        private static void EnsureCached(Type type)
        {
            if (!TypeCache.ContainsKey(type))
                BuildCache(type);
        }

        private static void BuildCache(Type type)
        {
            var groupToTabs = new Dictionary<string, List<string>>();
            var fieldToGroup = new Dictionary<string, string>();
            string currentGroup = null;

            var fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                var groupAttr = field.GetCustomAttribute<BeginTabGroupAttribute>();
                if (groupAttr != null)
                {
                    currentGroup = groupAttr.GroupId;
                    if (!groupToTabs.ContainsKey(currentGroup))
                        groupToTabs[currentGroup] = new List<string>();
                }

                var tabAttr = field.GetCustomAttribute<TabAttribute>();
                if (tabAttr != null && !string.IsNullOrEmpty(currentGroup))
                {
                    var tabs = groupToTabs[currentGroup];
                    if (!tabs.Contains(tabAttr.Tab))
                        tabs.Add(tabAttr.Tab);

                    fieldToGroup[field.Name] = currentGroup;
                }
            }

            TypeCache[type] = new GroupData
            {
                GroupToTabs = groupToTabs,
                FieldToGroup = fieldToGroup,
            };
        }

        public static void ClearCache()
        {
            TypeCache.Clear();
        }

        [InitializeOnLoadMethod]
        private static void ClearCachesOnDomainReload()
        {
            ClearCache();
        }
    }
}