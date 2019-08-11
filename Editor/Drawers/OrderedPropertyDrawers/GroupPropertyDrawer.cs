using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class GroupPropertyDrawer : OrderedPropertyDrawer<GroupAttribute>
    {
        //TODO: group data serialization based on single data structure(InspectorGroup class);

        /// <summary>
        /// Collection all grouped properties by single group name.
        /// </summary>
        private Dictionary<string, List<SerializedProperty>> groupedLists = new Dictionary<string, List<SerializedProperty>>();

        /// <summary>
        /// Collection of all grouped properties by group name.
        /// </summary>                                                                          
        private Dictionary<string, string> groupedProperties = new Dictionary<string, string>();

        /// <summary>
        /// Collection of all associated toggles used in group creation.
        /// </summary>
        private Dictionary<string, bool> groupedBools = new Dictionary<string, bool>();

        /// <summary>
        /// Creates inspector group using provided name and listed properties.
        /// </summary>
        /// <param name="groupName">Group name to display.</param>
        private void CreateGroup(string groupName)
        {
            EditorGUILayout.BeginVertical(Style.sectionStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            groupedBools[groupName] = EditorGUILayout.Foldout(groupedBools[groupName], groupName, Style.foldoutStyle);
            EditorGUILayout.EndHorizontal();
            if (groupedBools[groupName])
            {
                groupedLists[groupName].ForEach(property => base.DrawCustomProperty(property));
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }


        public GroupPropertyDrawer() : base(null)
        { }

        public GroupPropertyDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        {
            targetProperties.ForEach(property =>
            {
                var groupName = property.GetAttribute<GroupAttribute>().GroupName;
                if (groupedLists.ContainsKey(groupName))
                {
                    groupedLists[groupName].Add(property);
                }
                else
                {
                    groupedLists[groupName] = new List<SerializedProperty>()
                    {
                        property
                    };
                    groupedBools.Add(groupName, true);
                }
                groupedProperties.Add(property.name, groupName);
            });
        }


        /// <summary>
        /// Drawer method handled by ancestor class.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        protected override void DrawCustomProperty(SerializedProperty property)
        {
            if (groupedProperties.ContainsKey(property.name))
            {
                var groupName = groupedProperties[property.name];
                if (groupedLists[groupName].FirstOrDefault().name == property.name)
                {
                    CreateGroup(groupName);
                }
                return;
            }
            base.DrawCustomProperty(property);
        }


        /// <summary>
        /// Custom styling class.
        /// </summary>
        private static class Style
        {
            internal static GUIStyle sectionStyle;
            internal static GUIStyle foldoutStyle;

            static Style()
            {
                sectionStyle = new GUIStyle(GUI.skin.box);
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}