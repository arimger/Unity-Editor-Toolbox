using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class HelpDecorator : BaseMaterialPropertyDrawer
    {
        private readonly string message;
        private readonly MessageType type;


        public HelpDecorator(string message) : this(message, 1)
        {
            this.message = message;
        }

        public HelpDecorator(string message, float type)
        {
            this.message = message;
            this.type = (MessageType)type;
        }


        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            var minHeight = 24.0f;
            var calcHeight = Style.textStyle.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth);
            return Mathf.Max(minHeight, calcHeight);
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.HelpBox(position, message, type);
        }


        private static class Style
        {
            internal static readonly GUIStyle textStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
        }
    }
}