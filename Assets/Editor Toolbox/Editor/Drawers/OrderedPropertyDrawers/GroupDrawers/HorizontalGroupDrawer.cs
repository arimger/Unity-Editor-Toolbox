using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

//TODO: adjust label widths/add additional options;

namespace Toolbox.Editor
{
    public class HorizontalGroupDrawer : OrderedGroupDrawer<HorizontalGroupAttribute>
    {
        public HorizontalGroupDrawer() : base(null)
        { }

        public HorizontalGroupDrawer(List<SerializedProperty> componentProperties) : base(componentProperties)
        { }


        protected override void OnBeginGroup(HorizontalGroupAttribute attribute)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(attribute.GroupName, Style.headerStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
        }

        protected override void OnEndGroup(HorizontalGroupAttribute attribute)
        {
            EditorGUILayout.EndHorizontal();
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