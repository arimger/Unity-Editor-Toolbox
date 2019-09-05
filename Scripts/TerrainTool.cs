using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Toolbox
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    [AddComponentMenu("Toolbox/Terrain Tool", 1)]
    public class TerrainTool : MonoBehaviour
    {
        [SerializeField]
        private LayerMask terrainLayer;

        [SerializeField]
        private GameObject terrain;

        [SerializeField, AssetPreview]
        private List<GameObject> brushPrefabs;


        public void AddBrushPrefab(GameObject prefab)
        {
            if (brushPrefabs == null) brushPrefabs = new List<GameObject>();
            brushPrefabs.Add(prefab);
        }

        public void RemoveBrushPrefab(GameObject prefab)
        {
            brushPrefabs?.Remove(prefab);
        }

        public void MassPlacePrefabs(int count)
        {
            MassPlace(count, brushPrefabs.ToArray());
        }

        public void MassPlace(int count, params GameObject[] objects)
        {
            throw new NotImplementedException();
        }

        public void PlacePrefabs(Vector3 center, float gridSize, float radius, float density, LayerMask layer)
        {
            PlaceObjects(center, gridSize, radius, density, layer, brushPrefabs.ToArray());
        }

        public void PlaceObjects(Vector3 center, float gridSize, float radius, float density, LayerMask layer, params GameObject[] objects)
        {
            if (objects == null || objects.Length == 0)
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
                var prefab = objects[Random.Range(0, objects.Length)];
                var radians = Random.Range(0, 359) * Mathf.Deg2Rad;
                var distance = Random.Range(0.0f, radius);
                var rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
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

                if (Physics.Raycast(position + Vector3.up, Vector3.down, Mathf.Infinity, layer))
                {
                    var gameObject = Instantiate(prefab, terrain.transform);
                    gameObject.transform.position = position;
                    gameObject.transform.rotation = rotation;
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

        public GameObject[] Trees
        {
            get => brushPrefabs.ToArray();
        }
    }
}