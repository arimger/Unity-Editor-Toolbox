using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;
using PixelCoor = UnityEngine.Vector2Int;

namespace Toolbox.Editor.Tools
{
    public sealed class ViewGenerator : ScriptableWizard
    {
        private const string assetSaveLocation = "Assets/ToolAssets/Views/";

        #region Inspector fields

        public string objectName;

        [Separator]

        [BoxedToggle("Use Circle Shape")]
        public bool isCircled = true;
        [BoxedToggle("Use Grid Separation")]
        public bool isGridded;
        [BoxedToggle("Use Outline")]
        public bool isOutlined;

        [Separator]

        [Tooltip("Offset between sprite bounds and true image.")]
        public int spriteOffset = 10;
        public int pixelsPerUnit = 100;

        [ConditionalField("isGridded", true)]
        public int gridOffset = 15;
        [ConditionalField("isOutlined", true)]
        public int outlineSize = 6;

        public float radius = 1.0f;

        [Separator]

        public Color viewColor = new Color(1, 1, 1, 0.15f);
        [ConditionalField("isOutlined", true)]
        public Color outlineColor = new Color(1, 1, 1, 1);

        [Space]

        public Material material;

        #endregion


        [MenuItem("GameObject/Toolbox/Field of View", priority = 0)]
        private static void CreateWizard()
        {
            if (!Directory.Exists(assetSaveLocation))
            {
                Directory.CreateDirectory(assetSaveLocation);
            }

            DisplayWizard("Generate Field of View", typeof(ViewGenerator));
        }

        /// <summary>
        /// Inspector data validation.
        /// </summary>
        private void OnWizardUpdate()
        {
            gridOffset = Mathf.Max(gridOffset, 0);
            outlineSize = Mathf.Max(outlineSize, 0);
            spriteOffset = Mathf.Max(spriteOffset, 0);
            pixelsPerUnit = Mathf.Max(pixelsPerUnit, 0);

            radius = Mathf.Clamp(radius, 0.25f, 50f);
        }

        /// <summary>
        /// Asset creation.
        /// </summary>
        private void OnWizardCreate()
        {
            //setting atlas
            var atlasWidth = pixelsPerUnit * (int)((radius + 0.5) * 2);
            var atlasHeight = pixelsPerUnit * (int)((radius + 0.5) * 2);
            var atlas = new Texture2D(atlasWidth + spriteOffset, atlasHeight + spriteOffset, TextureFormat.RGBA32, false);
            //clearing atlas
            for (var i = 0; i < atlas.width; i++)
            {
                for (var j = 0; j < atlas.height; j++)
                {
                    atlas.SetPixel(i, j, Color.clear);
                }
            }

            if (isCircled)
            {
                CreateCircle(atlas, spriteOffset);
            }
            else
            {
                CreateSquare(atlas, spriteOffset);
            }

            if (isGridded)
            {
                throw new NotImplementedException();
            }

            if (isOutlined)
            {
                CreateOutline(atlas);
            }

            //final sprite creation
            var sprite = Sprite.Create(atlas, new Rect(0, 0, atlas.width, atlas.height), new Vector2(0.5f, 0.5f));

            //setting scene object
            var view = new GameObject
            {
                name = string.IsNullOrEmpty(objectName) ? "View" : objectName
            };
            view.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            view.transform.position = new Vector3(0, 0, 0);

            //setting basic components
            var collider = view.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = radius;
            var renderer = view.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetUtility.SaveSpriteToEditorPath(sprite, assetSaveLocation + GenerateAssetName());
            renderer.material = material ? material : new Material(Shader.Find("Sprites/Default"));
        }


        /// <summary>
        /// Creates view as single square.
        /// </summary>
        /// <param name="atlas"></param>
        private void CreateSquare(Texture2D atlas, int spriteOffset)
        {
            for (var i = spriteOffset / 2; i < atlas.width - spriteOffset / 2; i++)
            {
                for (var j = spriteOffset / 2; j < atlas.height - spriteOffset / 2; j++)
                {
                    atlas.SetPixel(i , j, viewColor);
                }
            }

            atlas.Apply();
        }

        /// <summary>
        /// Creates view as voxeled circle.
        /// </summary>
        /// <param name="atlas"></param>
        private void CreateCircle(Texture2D atlas, int spriteOffset)
        {
            var atlasWidth = pixelsPerUnit * (int)((radius + 0.5) * 2);
            var atlasHeight = pixelsPerUnit * (int)((radius + 0.5) * 2);
            //every sprite is divided by specific 2D voxels(pixelsPerUnit, pixelsPerUnit)
            //       __ __ __
            //      |__|__|__|
            //      |__|__|__|
            //      |__|__|__|
            var segmentsInRange = new bool[atlasHeight / pixelsPerUnit * atlasWidth / pixelsPerUnit];

            var segmentXIndex = 0;
            var segmentYIndex = 0;
            var segment1DIndex = 0;

            for (var i = 0; i < atlasWidth; i++)
            {
                for (var j = 0; j < atlasHeight; j++)
                {    
                    segmentXIndex = i / pixelsPerUnit;
                    segmentYIndex = j / pixelsPerUnit;
                    segment1DIndex = segmentXIndex * atlasHeight / pixelsPerUnit + segmentYIndex;
                    //skip previously visited segment
                    if (segmentsInRange[segment1DIndex]) continue;

                    //checking if current pixel coordinates are in raw range
                    if (PixelCoor.Distance(new PixelCoor(i, j), new PixelCoor(atlasWidth / 2, atlasHeight / 2)) / pixelsPerUnit <= radius)
                    {
                        //marking segment as "visited"
                        segmentsInRange[segment1DIndex] = true;
                        //painting all pixels in visited segment
                        //       __ __ __
                        //      |__|__|__|
                        //      ||||__|__|
                        //      ||||__|__|
                        for (var x = segmentXIndex * pixelsPerUnit; x < segmentXIndex * pixelsPerUnit + pixelsPerUnit; x++)
                        {
                            for (var y = segmentYIndex * pixelsPerUnit; y < segmentYIndex * pixelsPerUnit + pixelsPerUnit; y++)
                            {
                                atlas.SetPixel(x + spriteOffset / 2, y + spriteOffset / 2, viewColor);
                            }
                        }
                    }
                    //set clear pixel if coordinates are out of range
                    else
                    {
                        atlas.SetPixel(i + spriteOffset / 2, j + spriteOffset / 2, Color.clear);
                    }
                }
            }

            atlas.Apply();
        }

        /// <summary>
        /// Creates outline for desired atlas.
        /// </summary>
        /// <param name="atlas"></param>
        private void CreateOutline(Texture2D atlas)
        {
            var outlinePixels = new List<PixelCoor>();
            //collecting all pixels to outline
            for (var i = 0; i < atlas.width; i++)
            {
                for (var j = 0; j < atlas.height; j++)
                {
                    if (atlas.GetPixel(i, j) != Color.clear) continue;

                    if (atlas.GetPixel(Mathf.Max(0, i - 1), j) != Color.clear)
                    {
                        outlinePixels.Add(new PixelCoor(Mathf.Max(0, i - 1), j));
                    }
                    if (atlas.GetPixel(Mathf.Min(atlas.width, i + 1), j) != Color.clear)
                    {
                        outlinePixels.Add(new PixelCoor(Mathf.Min(atlas.width, i + 1), j));
                    }
                    if (atlas.GetPixel(i, Mathf.Max(0, j - 1)) != Color.clear)
                    {
                        outlinePixels.Add(new PixelCoor(i, Mathf.Max(0, j - 1)));
                    }
                    if (atlas.GetPixel(i, Mathf.Min(atlas.height, j + 1)) != Color.clear)
                    {
                        outlinePixels.Add(new PixelCoor(i, Mathf.Min(atlas.height, j + 1)));
                    }
                }
            }

            //painting all outlined pixels
            for (var i = 0; i < outlinePixels.Count; i++)
            {
                for (var j = outlinePixels[i].x - outlineSize / 2; j < outlinePixels[i].x + outlineSize / 2; j++)
                {
                    for (var k = outlinePixels[i].y - outlineSize / 2; k < outlinePixels[i].y + outlineSize / 2; k++)
                    {
                        atlas.SetPixel(j, k, outlineColor);
                    }
                }
            }

            atlas.Apply();
        }


        /// <summary>
        /// Generates Sprite Asset name using inspector fields.
        /// </summary>
        /// <returns>Returns desired asset name.</returns>
        private string GenerateAssetName()
        {
            return (string.IsNullOrEmpty(objectName) ? "View" : objectName) + "x"
                + radius.ToString()
                    .Replace(",", "")
                    .Replace(".", "")
                + (isOutlined ? "xOutline" : "") + (isGridded ? "xGrid" : "") + ".png";
        }
    }
}