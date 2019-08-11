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
        private List<GameObject> treePrefabs;
        [SerializeField, AssetPreview]
        private List<GameObject> bushPrefabs;


        public void AddTreePrefab(GameObject prefab)
        {
            if (treePrefabs == null) treePrefabs = new List<GameObject>();
            treePrefabs.Add(prefab);
        }

        public void RemoveTreePrefab(GameObject prefab)
        {
            treePrefabs?.Remove(prefab);
        }

        public void AddBushPrefab(GameObject prefab)
        {
            if (bushPrefabs == null) bushPrefabs = new List<GameObject>();
            bushPrefabs.Add(prefab);
        }

        public void RemoveBushPrefab(GameObject prefab)
        {
            bushPrefabs?.Remove(prefab);
        }

        public void MassPlaceTrees(int count)
        {
            MassPlace(count, treePrefabs.ToArray());
        }

        public void MassPlaceBushes(int count)
        {
            MassPlace(count, bushPrefabs.ToArray());
        }

        public void MassPlace(int count, params GameObject[] objects)
        {
            throw new NotImplementedException();
        }

        public void PlaceTrees(Vector3 center, float gridSize, float radius, float density, LayerMask layer)
        {
            PlaceObjects(center, gridSize, radius, density, layer, treePrefabs.ToArray());
        }

        public void PlaceBushes(Vector3 center, float gridSize, float radius, float density, LayerMask layer)
        {
            PlaceObjects(center, gridSize, radius, density, layer, bushPrefabs.ToArray());
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
            get => treePrefabs.ToArray();
        }

        public GameObject[] Bushes
        {
            get => bushPrefabs.ToArray();
        }
    }
}