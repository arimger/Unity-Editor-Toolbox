using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class TitleDecorator : BaseMaterialPropertyDrawer
    {
        private readonly GUIContent header;
        private readonly float spacing;


        public TitleDecorator(string header) : this(header, 4.0f)
        { }

        public TitleDecorator(string header, float spacing)
        {
            this.header = new GUIContent(header);
            this.spacing = spacing;
        }


        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return EditorGUIUtility.singleLineHeight + spacing;
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            position.yMin = position.yMax - EditorGUIUtility.singleLineHeight;
            ToolboxEditorGui.BoldLabel(position, header);
            position.yMin = position.yMax - EditorGUIUtility.standardVerticalSpacing;
            ToolboxEditorGui.DrawLine(position, padding: 0);
        }
    }
}