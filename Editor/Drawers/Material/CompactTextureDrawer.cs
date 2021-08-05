using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class CompactTextureDrawer : BaseMaterialPropertyDrawer
    {
        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 0;
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            editor.TexturePropertySingleLine(new GUIContent(label), prop);
        }

        protected override bool IsPropertyValid(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Texture;
        }
    }
}