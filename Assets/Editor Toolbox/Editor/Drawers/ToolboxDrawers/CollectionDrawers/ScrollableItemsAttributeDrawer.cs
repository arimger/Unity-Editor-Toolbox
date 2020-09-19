using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class ScrollableItemsAttributeDrawer : ToolboxListPropertyDrawer<ScrollableItemsAttribute>
    {
        private Vector2 scrollPosition;

        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ScrollableItemsAttribute attribute)
        {
            if (EditorGUILayout.PropertyField(property, label, false))
            {
                var size = property.GetSize();

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(size);
                EditorGUI.indentLevel--;

                var option = GUILayout.Height(attribute.AreaHeight);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, Style.backgroundStyle, option);
                if (size.longValue > 0)
                {
                    for (var i = 0; i < size.longValue; i++)
                    {
                        EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
                    }
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("List is empty", Style.emptyLabelStyle);
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private static class Style
        {
            internal static readonly GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
            internal static readonly GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.verticalScrollbar);
            internal static readonly GUIStyle emptyLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}