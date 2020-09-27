using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ScrollableItemsAttributeDrawer : ToolboxListPropertyDrawer<ScrollableItemsAttribute>
    {
        /// <summary>
        /// All cached ranges related to particular properties.
        /// </summary>
        private static Dictionary<string, Vector2> indexRanges = new Dictionary<string, Vector2>();


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ScrollableItemsAttribute attribute)
        {
            var propertyKey = property.GetPropertyTypeKey();
            //try to get previously cached scroll position
            if (!indexRanges.TryGetValue(propertyKey, out var indexRange))
            {
                var x = attribute.DefaultMinIndex;
                var y = attribute.DefaultMaxIndex;
                //var x = EditorPrefs.GetFloat(propertyKey + ".x", attribute.DefaultMinIndex);
                //var y = EditorPrefs.GetFloat(propertyKey + ".y", attribute.DefaultMaxIndex);
                indexRanges[propertyKey] = indexRange = new Vector2(x, y);
            }

            if (!EditorGUILayout.PropertyField(property, label, false))
            {
                return;
            }

            var size = property.GetSize();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(size);
            //create a min-max slider to determine the range of visible properties
            {
                var enabled = GUI.enabled;
                GUI.enabled = true;
                EditorGUILayout.MinMaxSlider(Style.rangeContent, ref indexRange.x, ref indexRange.y, 0, size.longValue);
                GUI.enabled = enabled;

                //NOTE: should we keep data between editors?
                //EditorPrefs.SetFloat(propertyKey, x);
                //EditorPrefs.SetFloat(propertyKey, y);
            }

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

            var minRange = (int)indexRange.x;
            var maxRange = (int)indexRange.y;

            if (minRange > 0)
            {
                EditorGUILayout.LabelField(Style.spaceContent, Style.spaceLabelStyle);
            }

            //draw all visible (in the range) properties
            for (var i = minRange; i < maxRange; i++)
            {
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), true);
            }

            if (maxRange < size.longValue)
            {
                EditorGUILayout.LabelField(Style.spaceContent, Style.spaceLabelStyle);
            }

            GUILayout.Space(space);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
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
            internal static readonly GUIStyle spaceLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            internal static readonly GUIContent spaceContent = new GUIContent("...");
            internal static readonly GUIContent rangeContent = new GUIContent("Min/Max", "Range of the min. and max. visible element.");
        }
    }
}