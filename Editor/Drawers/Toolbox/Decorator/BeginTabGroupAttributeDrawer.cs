using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public sealed class BeginTabGroupAttributeDrawer : ToolboxDecoratorDrawer<BeginTabGroupAttribute>
    {
        #region CONSTANTS
        private const float ViewWidthPadding = 32f;
        private const float TabSpacing = 8f;
        private const float MinTabWidth = 40f;
        private const float TabHeight = 22f;
        private const float RowSpacing = 4f;
        private const float TopSpacing = 2f;

        //TODO: just make as separate color
        private static readonly Color InactiveBgMultiplier = new(0.6f, 0.6f, 0.6f, 0.6f);

        //TODO: move to utility
        private static Color ActiveBgColor => EditorGUIUtility.isProSkin
            ? new Color(0.22f, 0.22f, 0.22f)
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

            var tabs = TabDiscovery.GetTabsForGroup(targetType, attribute.GroupId);
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }

            InitializeDefaultTab(attribute.GroupId, tabs);

            var currentTab = GetActiveTab(attribute.GroupId, tabs);
            int currentIndex = tabs.IndexOf(currentTab);

            if (currentIndex == -1)
                currentIndex = 0;

            //TODO: GroupId + Optional Label
            DrawGroupHeader(attribute.GroupId);

            //TODO: temp, make it static
            var style = new GUIStyle(EditorStyles.helpBox);
            style.margin = new RectOffset(0, 0, 0, 0);
            style.contentOffset = Vector2.zero;
            style.border = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(1, 1, 1, 1);

            //TODO: use layout utility
            EditorGUILayout.BeginVertical(style);

            int newIndex = DrawResponsiveTabs(currentIndex, tabs);

            if (newIndex != currentIndex)
            {
                TabState.Set(attribute.GroupId, tabs[newIndex]);

                //TODO: is it needed?
                GUI.FocusControl(null);
                EditorGUIUtility.keyboardControl = 0;
                EditorWindow.focusedWindow?.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        private static void InitializeDefaultTab(string groupId, List<string> tabs)
        {
            if (!TabState.Has(groupId))
            {
                TabState.Set(groupId, tabs[0]);
            }
        }

        private static string GetActiveTab(string groupId, List<string> tabs)
        {
            if (TabState.TryGet(groupId, out var activeTab) && tabs.Contains(activeTab))
                return activeTab;

            return tabs[0];
        }

        private static void DrawGroupHeader(string groupId)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(groupId, HeaderStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(TopSpacing);
        }

        private static int DrawResponsiveTabs(int currentIndex, List<string> tabs)
        {
            float viewWidth = EditorGUIUtility.currentViewWidth - ViewWidthPadding;

            var tabWidths = CalculateTabWidths(tabs, ActiveTabStyle, viewWidth);
            var rows = DistributeTabsIntoRows(tabs, tabWidths, viewWidth);

            RotateRowsToShowActiveTabLast(rows, currentIndex);

            int newIndex = DrawTabRows(rows, currentIndex, tabs);

            return newIndex;
        }

        //TODO: remove it
        private static List<float> CalculateTabWidths(List<string> tabs, GUIStyle style, float viewWidth)
        {
            var tabWidths = new List<float>(tabs.Count);

            foreach (var tab in tabs)
            {
                var content = new GUIContent(tab);
                float width = style.CalcSize(content).x + TabSpacing;

                width = Mathf.Max(width, MinTabWidth);
                width = Mathf.Min(width, Mathf.Max(MinTabWidth, viewWidth - TabSpacing));

                tabWidths.Add(width);
            }

            return tabWidths;
        }

        private static List<List<int>> DistributeTabsIntoRows(
            List<string> tabs,
            List<float> tabWidths,
            float viewWidth
        )
        {
            var rows = new List<List<int>>();
            var currentRow = new List<int>();
            float rowAccumulator = 0f;

            for (int i = 0; i < tabs.Count; i++)
            {
                float width = tabWidths[i];

                if (currentRow.Count == 0)
                {
                    currentRow.Add(i);
                    rowAccumulator = width;
                    continue;
                }

                if (rowAccumulator + width > viewWidth)
                {
                    rows.Add(currentRow);
                    currentRow = new List<int> { i };
                    rowAccumulator = width;
                }
                else
                {
                    currentRow.Add(i);
                    rowAccumulator += width;
                }
            }

            if (currentRow.Count > 0)
                rows.Add(currentRow);

            return rows;
        }

        private static void RotateRowsToShowActiveTabLast(List<List<int>> rows, int currentIndex)
        {
            if (rows.Count <= 1)
                return;

            int activeRowIndex = FindRowContainingTab(rows, currentIndex);

            if (activeRowIndex == rows.Count - 1)
                return;

            var rotated = new List<List<int>>();

            for (int r = activeRowIndex + 1; r < rows.Count; r++)
                rotated.Add(rows[r]);

            for (int r = 0; r <= activeRowIndex; r++)
                rotated.Add(rows[r]);

            rows.Clear();
            rows.AddRange(rotated);
        }

        private static int FindRowContainingTab(List<List<int>> rows, int tabIndex)
        {
            for (int r = 0; r < rows.Count; r++)
            {
                if (rows[r].Contains(tabIndex))
                    return r;
            }
            return 0;
        }

        private static int DrawTabRows(List<List<int>> rows, int currentIndex, List<string> tabs)
        {
            int newIndex = currentIndex;
            using (new EditorGUILayout.VerticalScope())
            {
                for (int r = 0; r < rows.Count; r++)
                {
                    var row = rows[r];
                    GUILayout.BeginHorizontal();
                    newIndex = DrawTabButton(row, currentIndex, newIndex, tabs);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(RowSpacing);
            }

            return newIndex;
        }

        private static int DrawTabButton(List<int> row, int currentIndex, int newIndex, List<string> tabs)
        {
            for (int i = 0; i < row.Count; i++)
            {
                int tabIndex = row[i];
                bool isActive = tabIndex == currentIndex;
                var content = new GUIContent(tabs[tabIndex]);

                GUIStyle style = GetStyleForVisual(i, row.Count, isActive);

                Color prevBg = GUI.backgroundColor;

                GUI.backgroundColor = isActive
                    ? ActiveBgColor
                    : GUI.backgroundColor * InactiveBgMultiplier;
                style = isActive ? ActiveTabStyle : BaseTabStyle;
                bool pressed = GUILayout.Toggle(isActive, content, style);
                GUI.backgroundColor = prevBg;

                if (pressed && !isActive)
                {
                    newIndex = tabIndex;
                }
            }

            return newIndex;
        }

        private static GUIStyle GetStyleForVisual(int index, int count, bool isActive)
        {
            return isActive ? ActiveTabStyle : BaseTabStyle;
        }
    }

    public sealed class TabAttributeDrawer : ToolboxConditionDrawer<TabAttribute>
    {
        protected override PropertyCondition OnGuiValidateSafe(SerializedProperty property, TabAttribute attribute)
        {
            //TODO: better way to get the unique ID
            var targetType = property.serializedObject.targetObject.GetType();
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
            //TODO: use layout utility
            EditorGUILayout.EndVertical();
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

        public static List<string> GetTabsForGroup(Type type, string groupId)
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