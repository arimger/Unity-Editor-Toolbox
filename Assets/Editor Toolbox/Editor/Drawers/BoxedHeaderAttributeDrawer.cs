using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(BoxedHeaderAttribute))]
    public class BoxedHeaderAttributeDrawer : ToolboxNativeDecoratorDrawer
    {
        public override float GetHeight()
        {
            return Style.height;
        }

        public override void OnGUI(Rect position)
        {
            position.height -= Style.spacing;

            //caching disabled group(prevent header from disabling)
            var cached = GUI.enabled;
            GUI.enabled = true;
            EditorGUI.LabelField(position, new GUIContent(Attribute.Header), Style.headerStyle);
            GUI.enabled = cached;
        }


        /// <summary>
        /// A wrapper which returns the PropertyDrawer.attribute field as a <see cref="BoxedHeaderAttribute"/>.
        /// </summary>
        private BoxedHeaderAttribute Attribute => attribute as BoxedHeaderAttribute;


        /// <summary>
        /// Static representation of header style.
        /// </summary>
        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

            internal static GUIStyle headerStyle;

            static Style()
            {
                headerStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
            }
        }
    }
}