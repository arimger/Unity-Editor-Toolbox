using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    public static class AssetUtility
    {
        public static Sprite SaveSpriteToEditorPath(Sprite sprite, string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (dir == null) Directory.CreateDirectory(dir);

            File.WriteAllBytes(path, sprite.texture.EncodeToPNG());
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            importer.alphaIsTransparency = true;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = sprite.pixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
        }

        public static Texture2D GetPersistentTexture(Color textureColor)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, textureColor);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }
    }
}