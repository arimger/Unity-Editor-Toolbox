using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class IndentDrawer : BaseMaterialPropertyDrawer
    {
        private readonly int indent;


        public IndentDrawer() : this(1)
        { }

        public IndentDrawer(float indent)
        {
            this.indent = (int)indent;
        }


        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return base.GetPropertyHeightSafe(prop, label, editor);
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.indentLevel += indent;
            base.OnGUISafe(position, prop, label, editor);
            EditorGUI.indentLevel -= indent;
        }
    }
}
