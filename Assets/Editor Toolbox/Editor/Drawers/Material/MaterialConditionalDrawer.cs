using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class MaterialConditionalDrawer : BaseMaterialPropertyDrawer
    {
        protected readonly string togglePropertyName;

        protected MaterialConditionalDrawer(string togglePropertyName)
        {
            this.togglePropertyName = togglePropertyName;
        }

        private bool IsValidToggleType(MaterialProperty toggleProp)
        {
#if UNITY_6000_3_OR_NEWER
            return toggleProp.propertyType == UnityEngine.Rendering.ShaderPropertyType.Float ||
                toggleProp.propertyType == UnityEngine.Rendering.ShaderPropertyType.Range;
#else
            return toggleProp.type == MaterialProperty.PropType.Float || 
                toggleProp.type == MaterialProperty.PropType.Range;
#endif
        }

        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!HasToggle(prop))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (!IsVisible(prop))
            {
                return 0.0f;
            }

            return base.GetPropertyHeightSafe(prop, label, editor);
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!HasToggle(prop))
            {
                DrawInvalidDrawerLabel(position, string.Format("{0} has invalid toggle source", label));
                return;
            }

            var value = GetValue(prop);
            if (!IsVisible(value))
            {
                return;
            }

            base.OnGUISafe(position, prop, label, editor);
        }

        protected virtual bool HasToggle(MaterialProperty prop)
        {
            var targets = prop.targets;
            var toggle = MaterialEditor.GetMaterialProperty(targets, togglePropertyName);
            return toggle != null && IsValidToggleType(toggle);
        }

        protected virtual bool? GetValue(MaterialProperty prop)
        {
            var targets = prop.targets;
            var result = GetValue((Material)targets[0]);
            for (var i = 1; i < targets.Length; i++)
            {
                var nextResult = GetValue((Material)targets[i]);
                if (nextResult != result)
                {
                    return null;
                }
            }

            return result == 1.0f;
        }

        protected virtual float GetValue(Material target)
        {
            return target.GetFloat(togglePropertyName);
        }

        protected virtual bool IsVisible(MaterialProperty prop)
        {
            var value = GetValue(prop);
            return IsVisible(value);
        }

        protected abstract bool IsVisible(bool? value);
    }
}