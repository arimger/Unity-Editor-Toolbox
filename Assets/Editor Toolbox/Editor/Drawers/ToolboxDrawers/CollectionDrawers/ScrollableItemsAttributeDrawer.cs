using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ScrollableItemsAttributeDrawer : ToolboxListPropertyDrawer<ScrollableItemsAttribute>
    {
        private static Dictionary<string, Vector2> indexRanges = new Dictionary<string, Vector2>();


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ScrollableItemsAttribute attribute)
        {
            var propertyKey = property.GetPropertyKey();
            //try to get previously cached scroll position
            if (!indexRanges.TryGetValue(propertyKey, out var indexRange))
            {
                indexRanges[propertyKey] = indexRange = new Vector2(attribute.DefaultMinIndex, attribute.DefaultMaxIndex);
            }

            if (!EditorGUILayout.PropertyField(property, label, false))
            {
                return;
            }

            var size = property.GetSize();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(size);
            //create a min-max slider to determine the range of visible properties
            EditorGUILayout.MinMaxSlider(new GUIContent("Min/Max", "Set min. and max. visible element."),
                ref indexRange.x, ref indexRange.y, 0, size.longValue);

            //fix values to the integral part
            indexRange.x = Mathf.Max(Mathf.RoundToInt(indexRange.x), 0);
            indexRange.y = Mathf.Min(Mathf.RoundToInt(indexRange.y), size.longValue);
            indexRanges[propertyKey] = indexRange;
            EditorGUI.indentLevel--;

            if (size.longValue == 0 || indexRange.x - indexRange.y == 0)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            //NOTE1: we have to handle indentation internally
            //NOTE2: 15.0f - internal 'indentPerLevel' value
            GUILayout.Space(15.0f * EditorGUI.indentLevel);

            EditorGUILayout.BeginVertical(Style.backgroundStyle);
            var space = EditorGUIUtility.standardVerticalSpacing;
            GUILayout.Space(space);
            GUILayout.Space(space);

            //draw all visible (in the range) properties
            for (var i = (int)indexRange.x; i < (int)indexRange.y; i++)
            {
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), true);
            }

            GUILayout.Space(space);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        public override void OnGuiReload()
        {
            //NOTE: should we keep data between editors?
            indexRanges.Clear();
        }


        private static class Style
        {
#if UNITY_2019_3_OR_NEWER
            internal static readonly GUIStyle backgroundStyle = new GUIStyle(EditorStyles.helpBox)
#else
            internal static readonly GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box)
#endif
            {
                padding = new RectOffset(15, 0, 0, 0)
            };
            internal static readonly GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.verticalScrollbar);
            internal static readonly GUIStyle emptyLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}