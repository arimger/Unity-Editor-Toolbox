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
            EditorGUILayout.BeginVertical(Style.sectionStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(attribute.GroupName, Style.headerStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
        }

        protected override void OnEndGroup(GroupAttribute attribute)
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Custom group styling class.
        /// </summary>
        private static class Style
        {
            internal static GUIStyle headerStyle;
            internal static GUIStyle sectionStyle;
            internal static GUIStyle foldoutStyle;
   
            static Style()
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel);
                sectionStyle = new GUIStyle(GUI.skin.box);
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }
        }
    }
}