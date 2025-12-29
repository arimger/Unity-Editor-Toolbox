using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class MaterialVector2Drawer : BaseMaterialPropertyDrawer
    {
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
                EditorGUI.BeginChangeCheck();
                vectorValue = EditorGUI.Vector2Field(position, label, vectorValue);
                if (EditorGUI.EndChangeCheck())
                {
                    prop.vectorValue = vectorValue;
                }
            }
        }

        protected override bool IsPropertyValid(MaterialProperty prop)
        {
#if UNITY_6000_3_OR_NEWER
            return prop.propertyType == UnityEngine.Rendering.ShaderPropertyType.Vector;
#else
            return prop.type == MaterialProperty.PropType.Vector;
#endif
        }
    }
}