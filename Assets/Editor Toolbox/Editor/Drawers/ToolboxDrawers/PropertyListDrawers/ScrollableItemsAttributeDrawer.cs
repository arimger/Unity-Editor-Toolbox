using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ScrollableItemsAttributeDrawer : ToolboxListPropertyDrawer<ScrollableItemsAttribute>
    {
        static ScrollableItemsAttributeDrawer()
        {
            storage = new DrawerDataStorage<Vector2, ScrollableItemsAttribute>(true, (p, a) =>
            {
                return new Vector2(a.DefaultMinIndex, a.DefaultMaxIndex);
            });
        }

        private static readonly DrawerDataStorage<Vector2, ScrollableItemsAttribute> storage;


        private void DrawSettingsBody(SerializedProperty property, ScrollableItemsAttribute attribute, out int size, out Vector2 indexRange)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(property.GetSize());
            size = property.arraySize;
            //get or initialize current ranges 
            indexRange = storage.ReturnItem(property, attribute);

            //create a min-max slider to determine the range of visible properties
            {
                var enabled = GUI.enabled;
                GUI.enabled = true;
                EditorGUILayout.MinMaxSlider(Style.rangeContent, ref indexRange.x, ref indexRange.y, 0, size);
                GUI.enabled = enabled;
            }

            //fix values to the integral part
            indexRange.x = Mathf.Max(Mathf.RoundToInt(indexRange.x), 0);
            indexRange.y = Mathf.Min(Mathf.RoundToInt(indexRange.y), size);
            storage.ApplyItem(property, indexRange);
            EditorGUI.indentLevel--;
        }

        private void DrawElementsBody(SerializedProperty property, ScrollableItemsAttribute attribute, int size, Vector2 indexRange)
        {
            if (size == 0 || indexRange.x - indexRange.y == 0)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            //NOTE1: we have to handle indentation for group
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
                ToolboxEditorGui.DrawToolboxProperty(property.GetArrayElementAtIndex(i));
            }

            if (maxRange < size)
            {
                EditorGUILayout.LabelField(Style.spaceContent, Style.spaceLabelStyle);
            }


            GUILayout.Space(space);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ScrollableItemsAttribute attribute)
        {
            using (var propertyScope = new PropertyScope(property, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                DrawSettingsBody(property, attribute, out var size, out var indexRange);
                DrawElementsBody(property, attribute, size, indexRange);
            }
        }


        private static class Style
        {
#if UNITY_2019_3_OR_NEWER
            internal static readonly GUIStyle backgroundStyle = new GUIStyle("helpBox")
#else
            internal static readonly GUIStyle backgroundStyle = new GUIStyle("box")
#endif
            {
                padding = new RectOffset(15, 0, 0, 0)
            };
            internal static readonly GUIStyle scrollViewStyle = new GUIStyle("verticalScrollbar");
            internal static readonly GUIStyle spaceLabelStyle = new GUIStyle("label")
            {
                alignment = TextAnchor.MiddleCenter
            };

            internal static readonly GUIContent spaceContent = new GUIContent("...");
            internal static readonly GUIContent rangeContent = new GUIContent("Min/Max", "Range of the min. and max. visible element.");
        }
    }
}