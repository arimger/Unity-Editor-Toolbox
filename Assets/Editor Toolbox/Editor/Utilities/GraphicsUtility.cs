using UnityEngine;

namespace Toolbox.Editor
{
    internal static class GraphicsUtility
    {
        public static Texture2D CreatePersistantTexture()
        {
            return CreatePersistantTexture(Color.clear);
        }

        public static Texture2D CreatePersistantTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.filterMode = FilterMode.Point;
            texture.hideFlags = HideFlags.HideAndDontSave;
            texture.Apply();

            return texture;
        }
    }
}