using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public class BoxedGroupDrawer : OrderedGroupDrawer<GroupAttribute>
    {
        public BoxedGroupDrawer() : base(null)
        { }

        public BoxedGroupDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        protected override void OnBeginGroup(GroupAttribute attribute)
        {
            EditorGUILayout.BeginHorizontal(Style.headerBackgroundStyle);
            EditorGUILayout.LabelField(attribute.GroupName, Style.headerStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(-Style.spacing * 2);
            EditorGUILayout.BeginVertical(Style.sectionBackgroundStyle);
            GUILayout.Space(Style.spacing * 2);
            EditorGUI.indentLevel++;
        }

        protected override void OnEndGroup(GroupAttribute attribute)
        {
            EditorGUI.indentLevel--;
            GUILayout.Space(Style.spacing * 2);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Custom group styling class.
        /// </summary>
        private static class Style
        {
            internal static readonly float spacing = 2.5f;

            internal static GUIStyle headerStyle;
            internal static GUIStyle headerBackgroundStyle;
            internal static GUIStyle sectionBackgroundStyle;
            internal static GUIStyle foldoutStyle;
   
            static Style()
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                headerBackgroundStyle = new GUIStyle(GUI.skin.box);
                sectionBackgroundStyle = new GUIStyle(GUI.skin.box);
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}