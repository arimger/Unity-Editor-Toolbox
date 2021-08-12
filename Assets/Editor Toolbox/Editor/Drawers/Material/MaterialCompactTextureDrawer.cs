using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public class MaterialCompactTextureDrawer : BaseMaterialPropertyDrawer
    {
        private readonly string tooltip;


        public MaterialCompactTextureDrawer() : this(string.Empty)
        { }

        public MaterialCompactTextureDrawer(string tooltip)
        {
            this.tooltip = tooltip;
        }


        protected override float GetPropertyHeightSafe(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void OnGUISafe(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            editor.TexturePropertyMiniThumbnail(position, prop, label, tooltip);
        }

        protected override bool IsPropertyValid(MaterialProperty prop)
        {
            return prop.type == MaterialProperty.PropType.Texture;
        }
    }
}