using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Toolbox
{
    [Serializable]
    public class BrushPrefab
    {
        [AssetPreview]
        public GameObject target;

        public bool useRandomScale;
        public Vector3 minScale = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 maxScale;

        public bool useRandomRotation = true;
        public Vector3 minRotation;
        public Vector3 maxRotation = new Vector3(0.0f, 359.0f, 0.0f);
    }

    [ExecuteInEditMode, DisallowMultipleComponent]
    [AddComponentMenu("Toolbox/Terrain Tool", 1)]
    public class TerrainTool : MonoBehaviour
    {
        [SerializeField]
        private LayerMask terrainLayer;

        [SerializeField]
        private GameObject terrain;

        [SerializeField]
        private List<BrushPrefab> brushPrefabs;


        public void AddBrushPrefab(BrushPrefab prefab)
        {
            if (brushPrefabs == null) brushPrefabs = new List<BrushPrefab>();
            brushPrefabs.Add(prefab);
        }

        public void RemoveBrushPrefab(BrushPrefab prefab)
        {
            brushPrefabs?.Remove(prefab);
        }

        public void MassPlacePrefabs(int count)
        {
            MassPlace(count, brushPrefabs.ToArray());
        }

        public void MassPlace(int count, params BrushPrefab[] prefabs)
        {
            throw new NotImplementedException();
        }

        public void PlacePrefabs(Vector3 center, float gridSize, float radius, float density, LayerMask layer)
        {
            PlaceObjects(center, gridSize, radius, density, layer, brushPrefabs.ToArray());
        }

        public void PlaceObjects(Vector3 center, float gridSize, float radius, float density, LayerMask layer, params BrushPrefab[] prefabs)
        {
            if (prefabs == null || prefabs.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No objects to instantiate.");
#endif
                return;
            }

            //list of all created positions
            var grid = new List<Vector2>();
            //count calculation using circle area, provided density and single cell size
            var count = Mathf.Max((Mathf.PI * radius * radius * density) / gridSize, 1);

            for (var i = 0; i < count; i++)
            {
                var prefab = prefabs[Random.Range(0, prefabs.Length)];
                var radians = Random.Range(0, 359) * Mathf.Deg2Rad;
                var distance = Random.Range(0.0f, radius);
                var position = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * distance + center;
                var gridPosition = new Vector2(position.x - position.x % gridSize, position.z - position.z % gridSize);
                //position validation
                if (grid.Contains(gridPosition))
                {
                    continue;
                }
                else
                {
                    grid.Add(gridPosition);
                }

                if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, layer))
                {
                    var gameObject = Instantiate(prefab.target);
                    if (prefab.useRandomRotation)
                    {
                        gameObject.transform.eulerAngles = new Vector3(Random.Range(prefab.minRotation.x, prefab.maxRotation.x),
                            Random.Range(prefab.minRotation.y, prefab.maxRotation.y), Random.Range(prefab.minRotation.z, prefab.maxRotation.z));
                    }

                    if (prefab.useRandomScale)
                    {
                        gameObject.transform.localScale = new Vector3(Random.Range(prefab.minScale.x, prefab.maxScale.x),
                            Random.Range(prefab.minScale.y, prefab.maxScale.y), Random.Range(prefab.minScale.z, prefab.maxScale.z));
                    }

                    gameObject.transform.position = hitInfo.point;
                    gameObject.transform.parent = terrain.transform;
                }
            }
        }


        public LayerMask TerrainLayer
        {
            get => terrainLayer;
            set => terrainLayer = value;
        }

        public GameObject Terrain
        {
            get => terrain;
            set => terrain = value;
        }

        public BrushPrefab[] Trees
        {
            get => brushPrefabs.ToArray();
        }
    }
}