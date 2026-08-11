using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    internal static class TabsCacheManager
    {
        private class TabGroup
        {
            private readonly string id;
            private readonly List<string> tabs = new List<string>();

            private string activeTab;

            public TabGroup(string id)
            {
                this.id = id;
            }

            public bool GetIsTabActive(string tab)
            {
                return activeTab == tab;
            }

            public void SetIsTabActive(string tab)
            {
                activeTab = tab;
            }

            public bool ContainsTab(string tab)
            {
                return tabs.Contains(tab);
            }

            public void RegisterTab(string tab)
            {
                if (tabs.Contains(tab))
                {
                    return;
                }

                tabs.Add(tab);
                if (string.IsNullOrEmpty(activeTab))
                {
                    activeTab = tab;
                }
            }

            public string GetActiveTab(out int index)
            {
                if (!string.IsNullOrEmpty(activeTab))
                {
                    for (var i = 0; i < tabs.Count; i++)
                    {
                        var tab = tabs[i];
                        if (tab == activeTab)
                        {
                            index = i;
                            return tab;
                        }
                    }
                }

                index = -1;
                return null;
            }

            public string Id => id;
            public IReadOnlyList<string> Tabs => tabs;
        }

        private class TypeData
        {
            private readonly Dictionary<string, TabGroup> groupsByGroups;
            private readonly Dictionary<string, TabGroup> groupsByFields;

            public TypeData()
            {
                groupsByGroups = new Dictionary<string, TabGroup>();
                groupsByFields = new Dictionary<string, TabGroup>();
            }

            public void RegisterGroup(TabGroup group)
            {
                var groupId = group.Id;
                groupsByGroups[groupId] = group;
            }

            public void RegisterField(string fieldName, string tab, string groupId)
            {
                if (groupsByGroups.TryGetValue(groupId, out var group))
                {
                    group.RegisterTab(tab);
                    groupsByFields[fieldName] = group;
                }
            }

            public IReadOnlyList<string> GetTabsForGroup(string groupId)
            {
                return groupsByGroups.TryGetValue(groupId, out var group) ? group.Tabs : null;
            }

            public bool TryGetGroupForGroup(string groupId, out TabGroup group)
            {
                return groupsByGroups.TryGetValue(groupId, out group);
            }

            public bool TryGetGroupForField(string fieldName, out TabGroup group)
            {
                return groupsByFields.TryGetValue(fieldName, out group);
            }

            public bool ContainsGroup(string groupId)
            {
                return groupsByGroups.ContainsKey(groupId);
            }
        }

        private static readonly Dictionary<Type, TypeData> typeCache = new();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            ClearCache();
        }

        private static TypeData FetchTypeCache(Type type)
        {
            if (typeCache.TryGetValue(type, out var typeData))
            {
                return typeData;
            }
            else
            {
                return CreateTypeCache(type);
            }
        }

        private static TypeData CreateTypeCache(Type type)
        {
            var typeData = new TypeData();
            var typeName = type.Name;
            typeCache[type] = typeData;

            string currentGroup = null;

            var fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var fieldName = field.Name;

                var beginGroupAttribute = field.GetCustomAttribute<BeginTabGroupAttribute>();
                if (beginGroupAttribute != null)
                {
                    var groupId = beginGroupAttribute.GroupId;
                    if (string.IsNullOrEmpty(groupId))
                    {
                        ToolboxEditorLog.AttributeUsageWarning(typeof(BeginTabGroupAttribute), $"{typeName}: {fieldName}: Group ID is not specified.");
                    }
                    else if (typeData.ContainsGroup(groupId))
                    {
                        ToolboxEditorLog.AttributeUsageWarning(typeof(BeginTabGroupAttribute), $"{typeName}: Multiple {nameof(BeginTabGroupAttribute)} " +
                            $"have the same Group ID ('{groupId}'). This is not supported.");
                    }
                    else
                    {
                        currentGroup = groupId;
                        var group = new TabGroup(groupId);
                        typeData.RegisterGroup(group);
                    }
                }

                var tabAttribute = field.GetCustomAttribute<TabAttribute>();
                if (tabAttribute != null && !string.IsNullOrEmpty(currentGroup))
                {
                    var tab = tabAttribute.Tab;
                    if (string.IsNullOrEmpty(tab))
                    {
                        ToolboxEditorLog.AttributeUsageWarning(typeof(TabAttribute), $"{typeName}: {fieldName}: Tab name is not specified.");
                    }
                    else
                    {
                        typeData.RegisterField(fieldName, tab, currentGroup);
                    }
                }

                var endGroupAttribute = field.GetCustomAttribute<EndTabGroupAttribute>();
                if (endGroupAttribute != null)
                {
                    currentGroup = null;
                }
            }

            return typeData;
        }

        public static IReadOnlyList<string> GetTabsForGroup(Type declaringType, string groupId)
        {
            var typeData = FetchTypeCache(declaringType);
            return typeData.GetTabsForGroup(groupId);
        }

        public static bool TryGetIsTabActive(SerializedProperty serializedProperty, string targetTab, out bool isActive)
        {
            var declaringObject = serializedProperty.GetDeclaringObject();
            return TryGetIsTabActive(declaringObject?.GetType(), serializedProperty.name, targetTab, out isActive);
        }

        public static bool TryGetIsTabActive(Type declaringType, string fieldName, string targetTab, out bool isActive)
        {
            if (declaringType == null)
            {
                isActive = false;
                return false;
            }

            var typeData = FetchTypeCache(declaringType);
            if (!typeData.TryGetGroupForField(fieldName, out var group))
            {
                isActive = false;
                return false;
            }

            isActive = group.GetIsTabActive(targetTab);
            return true;
        }

        public static bool TrySetIsTabActive(Type declaringType, string groupId, string targetTab)
        {
            if (declaringType == null)
            {
                return false;
            }

            var typeData = FetchTypeCache(declaringType);
            if (!typeData.TryGetGroupForGroup(groupId, out var group))
            {
                return false;
            }

            group.SetIsTabActive(targetTab);
            return true;
        }

        public static bool TryGetActiveTabName(Type declaringType, string groupId, out string targetTab, out int activeIndex)
        {
            if (declaringType == null)
            {
                targetTab = null;
                activeIndex = -1;
                return false;
            }

            var typeData = FetchTypeCache(declaringType);
            if (!typeData.TryGetGroupForGroup(groupId, out var group))
            {
                targetTab = null;
                activeIndex = -1;
                return false;
            }

            targetTab = group.GetActiveTab(out activeIndex);
            return true;
        }

        public static void ClearCache()
        {
            typeCache.Clear();
        }
    }
}