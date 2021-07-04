using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ScrollableItemsAttributeDrawer : ToolboxListPropertyDrawer<ScrollableItemsAttribute>
    {
        static ScrollableItemsAttributeDrawer()
        {
            storage = new PropertyDataStorage<Vector2, ScrollableItemsAttribute>(true, (p, a) =>
            {
                return new Vector2(a.DefaultMinIndex, a.DefaultMaxIndex);
            });
        }

        private static readonly PropertyDataStorage<Vector2, ScrollableItemsAttribute> storage;


        private void DrawSettingsBody(SerializedProperty property, ScrollableItemsAttribute attribute, out int size, out Vector2 indexRange)
        {
            EditorGUILayout.PropertyField(property.GetSize());
            size = property.arraySize;
            //get or initialize current ranges 
            indexRange = storage.ReturnItem(property, attribute);

            using (new DisabledScope(true))
            {
                //create a min-max slider to determine the range of visible properties
                EditorGUILayout.MinMaxSlider(Style.rangeContent, ref indexRange.x, ref indexRange.y, 0, size);
            }

            //fix values to the integral part
            indexRange.x = Mathf.Max(Mathf.RoundToInt(indexRange.x), 0);
            indexRange.y = Mathf.Min(Mathf.RoundToInt(indexRange.y), size);
            storage.AppendItem(property, indexRange);
        }

        private void DrawElementsBody(SerializedProperty property, ScrollableItemsAttribute attribute, int size, Vector2 indexRange)
        {
            if (size == 0 || indexRange.x - indexRange.y == 0)
            {
                return;
            }

            var minRange = (int)indexRange.x;
            var maxRange = (int)indexRange.y;
            //draw all visible (in the range) properties
            for (var i = minRange; i < maxRange; i++)
            {
                ToolboxEditorGui.DrawToolboxProperty(property.GetArrayElementAtIndex(i));
            }
        }


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ScrollableItemsAttribute attribute)
        {
            using (var propertyScope = new PropertyScope(property, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                EditorGUI.indentLevel++;
                DrawSettingsBody(property, attribute, out var size, out var indexRange);
                DrawElementsBody(property, attribute, size, indexRange);
                EditorGUI.indentLevel--;
            }
        }


        private static class Style
        {
            //TODO: apply custom styling for the drawer
            internal static readonly GUIStyle scrollViewStyle = new GUIStyle("verticalScrollbar");
            internal static readonly GUIStyle spaceLabelStyle = new GUIStyle("label")
            {
                alignment = TextAnchor.MiddleCenter
            };
            internal static readonly GUIContent rangeContent = new GUIContent("Min/Max", "Range of the min. and max. visible element.");
        }
    }
}