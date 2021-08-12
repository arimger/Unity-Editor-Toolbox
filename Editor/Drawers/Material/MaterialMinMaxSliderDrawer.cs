using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class MaterialMinMaxSliderDrawer : BaseMaterialPropertyDrawer
    {
        private readonly float minValue;
        private readonly float maxValue;


        public MaterialMinMaxSliderDrawer(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }


        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            using (new FixedFieldsScope())
            {
                EditorGUIUtility.labelWidth = 0;

                var vectorValue = prop.vectorValue;
                var xValue = vectorValue.x;
                var yValue = vectorValue.y;

                EditorGUI.BeginChangeCheck();
                ToolboxEditorGui.DrawMinMaxSlider(position, label, ref xValue, ref yValue, minValue, maxValue);
                if (EditorGUI.EndChangeCheck())
                {
                    vectorValue.x = xValue;
                    vectorValue.y = yValue;
                    prop.vectorValue = vectorValue;
                }
            }
        }

        protected override bool IsPropertyValid(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Vector;
        }
    }
}