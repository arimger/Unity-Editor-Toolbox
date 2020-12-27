using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    public class HierarchyScriptDrawer : HierarchyDataDrawer
    {
        private float width;
        private bool isHighlighted;
        private Component[] components;


        public override void Prepare(GameObject target, Rect availableRect)
        {
            base.Prepare(target, availableRect);

            var rect = availableRect;
            rect.xMin = rect.xMax - Style.minWidth;
            if (rect.Contains(Event.current.mousePosition))
            {
                isHighlighted = true;
                components = target.GetComponents<Component>();
                width = components.Length > 1
                     ? (components.Length - 1) * Style.minWidth
                     : Style.minWidth;
            }
            else
            {
                isHighlighted = false;
                width = Style.minWidth;
            }
        }

        public override float GetWidth()
        {
            return width;
        }

        public override void OnGui(Rect rect)
        {
            var tooltip = string.Empty;
            var texture = Style.componentIcon;

            rect.xMin = rect.xMax - Style.minWidth;

            if (isHighlighted)
            {
                if (components.Length > 1)
                {
                    texture = null;
                    tooltip = "Components:\n";
                    //set tooltip based on available components
                    for (var i = 1; i < components.Length; i++)
                    {
                        tooltip += "- " + components[i].GetType().Name;
                        tooltip += i == components.Length - 1 ? "" : "\n";
                    }

                    //create tooltip for the basic rect
                    GUI.Label(rect, new GUIContent(string.Empty, tooltip));

                    //adjust rect to all component icons
                    rect.xMin -= Style.minWidth * (components.Length - 2);

                    var iconRect = rect;
                    iconRect.xMin = rect.xMin;
                    iconRect.xMax = rect.xMin + Style.minWidth;

                    //iterate over available components
                    for (var i = 1; i < components.Length; i++)
                    {
                        var component = components[i];
                        var content = EditorGUIUtility.ObjectContent(component, component.GetType());

                        //draw icon for the current component
                        GUI.Label(iconRect, new GUIContent(content.image));
                        //adjust rect for the next icon
                        iconRect.x += Style.minWidth;
                    }

                    return;
                }
                else
                {
                    texture = Style.transformIcon;
                    tooltip = "There is no additional component";
                }
            }

            GUI.Label(rect, new GUIContent(texture, tooltip));
        }


        private static class Style
        {
            internal static readonly float minWidth = 17.0f;

            internal static readonly Texture componentIcon;
            internal static readonly Texture transformIcon;

            static Style()
            {
                componentIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                transformIcon = EditorGUIUtility.IconContent("Transform Icon").image;
            }
        }
    }
}