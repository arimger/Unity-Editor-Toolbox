using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Tools
{
    public sealed class GridGenerator : ScriptableWizard
    {
        private const string assetSaveLocation = "Assets/ToolAssets/Grids/";

        #region Inspector fields

        public string objectName;

        [Separator]

        [Tooltip("Squared size.")]
        public int gridSize = 300;
        [Tooltip("Size of single cell.")]
        public int cellSize = 1;

        [Space]

        public Color32 gridColor = new Color32(128, 128, 128, 100);

        #endregion


        [MenuItem("GameObject/Toolbox/Grid", priority = 0)]
        private static void CreateWizard()
        {
            if (!Directory.Exists(assetSaveLocation))
            {
                Directory.CreateDirectory(assetSaveLocation);
            }

            DisplayWizard("Generate Grid", typeof(GridGenerator));
        }

        /// <summary>
        /// Inspector data validation.
        /// </summary>
        private void OnWizardUpdate()
        {
            gridSize = Mathf.Max(gridSize, 0);
            cellSize = Mathf.Max(cellSize, 0);
        }

        /// <summary>
        /// Asset creation.
        /// </summary>
        private void OnWizardCreate()
        {
            //scene object creation
            var gameObject = new GameObject
            {
                name = string.IsNullOrEmpty(objectName) ? "Grid" : objectName
            };
            gameObject.transform.localScale = new Vector3(gridSize, 1, gridSize);

            //add necessary components 
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Sprites/Default"))
            {
                color = gridColor
            };

            var mesh = AssetDatabase.LoadAssetAtPath(assetSaveLocation + GenerateAssetName(), typeof(Mesh)) as Mesh;
            //mesh creation if needed
            if (mesh == null)
            {
                mesh = GenerateGridedMesh();
                AssetDatabase.CreateAsset(mesh, assetSaveLocation + GenerateAssetName());
                AssetDatabase.SaveAssets();
            }

            meshFilter.mesh = mesh;
        }

        /// <summary>
        /// Generates grided mesh using inspector fields.
        /// </summary>
        /// <returns></returns>
        private Mesh GenerateGridedMesh()
        {
            var mesh = new Mesh();

            var i = 0;
            var step = (1 / ((float)gridSize / cellSize));

            var indices = new List<int>();
            var verticies = new List<Vector3>();

            for (float x = -0.5f; x <= 0.5f; x += step)
            {
                verticies.Add(new Vector3(x, 0.5f, -0.5f));
                verticies.Add(new Vector3(x, 0.5f, 0.5f));
                indices.Add(i);
                indices.Add(i + 1);
                i += 2;
            }

            for (float y = -0.5f; y <= 0.5f; y += step)
            {
                verticies.Add(new Vector3(-0.5f, 0.5f, y));
                verticies.Add(new Vector3(0.5f, 0.5f, y));
                indices.Add(i);
                indices.Add(i + 1);
                i += 2;
            }

            mesh.SetVertices(verticies);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            return mesh;
        }

        /// <summary>
        /// Generates asset name using inspector fields.
        /// </summary>
        /// <returns>Returns desired asset name.</returns>
        private string GenerateAssetName()
        {
            return (string.IsNullOrEmpty(objectName) ? "Grid" : objectName) + "x" + gridSize + "x" + cellSize + ".asset";
        }
    }
}