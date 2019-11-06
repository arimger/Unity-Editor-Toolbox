using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using UnityTools = UnityEditor.Tools;

//TODO: environment tool;

namespace Toolbox.Editor.Tools.Editors
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Editor for <see cref="TerrainTool"/> component. Provides tools to manipulate game environment.
    /// </summary>
    [CustomEditor(typeof(TerrainTool), true, isFallback = false)]
    public sealed class TerrainToolEditor : ToolEditor
    {
        private static bool settingsSectionToggle = true;
        private static bool paintingToolToggle;
        private static bool paintingSectionToggle = true;

        private float gridSize = 1.0f;
        private float brushRadius = 8.0f;
        private float brushDensity = 0.5f;

        private Color brushColor = new Color(0, 0, 0, 0.4f);

        private SerializedProperty layerProperty;
        private SerializedProperty terrainProperty;
        private SerializedProperty brushPrefabsProperty;

        private ReorderableList brushPrefabsList;


        /// <summary>
        /// Tool initialization.
        /// </summary>
        protected override void OnEnable()
        {
            //set proper toggles
            settingsSectionToggle = EditorPrefs.GetBool("TerrainEditor.settingsSectionToggle", settingsSectionToggle);
            paintingSectionToggle = EditorPrefs.GetBool("TerrainEditor.paintingSectionToggle", paintingSectionToggle);

            //set proper brush settings
            gridSize = EditorPrefs.GetFloat("TerrainEditor.gridSize", gridSize);
            brushRadius = EditorPrefs.GetFloat("TerrainEditor.brushRadius", brushRadius);
            brushDensity = EditorPrefs.GetFloat("TerrainEditor.brushDensity", brushDensity);

            var r = EditorPrefs.GetFloat("TerrainEditor.brushColor.r", brushColor.r);
            var g = EditorPrefs.GetFloat("TerrainEditor.brushColor.g", brushColor.g);
            var b = EditorPrefs.GetFloat("TerrainEditor.brushColor.b", brushColor.b);
            var a = EditorPrefs.GetFloat("TerrainEditor.brushColor.a", brushColor.a);
            brushColor = new Color(r, g, b, a);

            //basic editor initialization
            base.OnEnable();

            //get all needed and serialized properties from component
            layerProperty = serializedObject.FindProperty("terrainLayer");
            terrainProperty = serializedObject.FindProperty("terrain");
            brushPrefabsProperty = serializedObject.FindProperty("brushPrefabs");

            //creation of prefabs list
            brushPrefabsList = ToolboxEditorGui.CreateLinedList(brushPrefabsProperty, "Item");
        }

        /// <summary>
        /// Tool deinitialization.
        /// </summary>
        protected override void OnDisable()
        {
            paintingToolToggle = false;

            base.OnDisable();

            EditorPrefs.SetBool("TerrainEditor.settingsSectionToggle", settingsSectionToggle);
            EditorPrefs.SetBool("TerrainEditor.paintingSectionToggle", paintingSectionToggle);

            EditorPrefs.SetFloat("TerrainEditor.gridSize", gridSize);
            EditorPrefs.SetFloat("TerrainEditor.brushRadius", brushRadius);
            EditorPrefs.SetFloat("TerrainEditor.brushDensity", brushDensity);

            EditorPrefs.SetFloat("TerrainEditor.brushColor.r", brushColor.r);
            EditorPrefs.SetFloat("TerrainEditor.brushColor.g", brushColor.g);
            EditorPrefs.SetFloat("TerrainEditor.brushColor.b", brushColor.b);
            EditorPrefs.SetFloat("TerrainEditor.brushColor.a", brushColor.a);
        }


        /// <summary>
        /// Scene view managment used in tool functionality.
        /// </summary>
        private void OnSceneGUI()
        {
            if (!paintingToolToggle)
            {
                return;
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (UnityTools.current != Tool.None)
            {
                UnityTools.current = Tool.None;
            }

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            //check for possible surfaces using provided terrain layer
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Target.TerrainLayer))
            {
                Handles.color = brushColor;
                //brush visualization using solid arc
                Handles.DrawSolidArc(hit.point, Vector3.up, Vector3.right, 360, brushRadius);

                if (Event.current.type == EventType.MouseDown)
                {
                    if (Event.current.button == 0)
                    {
                        if (paintingToolToggle)
                        {
                            PlaceObjectsInBrush(hit.point, gridSize, brushRadius, brushDensity, Target.TerrainLayer, Target.Terrain.transform, Target.Trees);
                        }
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }
                }
            }

            SceneView.RepaintAll();
        }

        /// <summary>
        /// Draws all functional buttons(tool toggles).
        /// </summary>
        private void DrawButtonsStrip()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //setting functional toggle group
            paintingToolToggle = GUILayout.Toggle(paintingToolToggle, Style.brushToggleContent, Style.toolToggleStyle, Style.toggleOptions);
            //TODO: environment tool;
            GUILayout.Toggle(false, Style.terrainToggleContent, Style.toolToggleStyle, Style.toggleOptions);
            GUILayout.Toggle(false, Style.noneToggleContent, Style.toolToggleStyle, Style.toggleOptions);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (!paintingToolToggle) return;

            EditorGUILayout.HelpBox("Tool active \n\n" +
                                    "Navigate mouse to desired terrain and press left button to create objects \n\n" +
                                    "Ctrl+Z - Undo", MessageType.Info);
        }

        /// <summary>
        /// Draws tool section(options) based on <see cref="ReorderableList"/> of prefabs.
        /// </summary>
        private void DrawToolSection()
        {
            if (!paintingToolToggle) return;

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            if (Style.brushIcon)
            {
                GUILayout.Label(Style.brushIcon, Style.smallIconStyle, Style.smallIconOptions);
                GUILayout.Space(10);
            }
            paintingSectionToggle = EditorGUILayout.Foldout(paintingSectionToggle, "Brush Options", Style.headerToggleStyle);
            GUILayout.EndHorizontal();

            if (paintingSectionToggle)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                brushPrefabsList.DoLayoutList();

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Button("Reset", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(50));
                if (GUILayout.Button("Mass Place", EditorStyles.miniButtonRight, GUILayout.MaxWidth(100)))
                {
                    if (MassPlace(0))
                    {
                        Debug.LogWarning("Not implemented yet.");
                    }
                }
                EditorGUILayout.IntField(0, GUILayout.MaxWidth(30));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws possible settings in grouped form.
        /// </summary>
        private void DrawSettingsSection()
        {
            if (!terrainProperty.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Assign Terrain property.", MessageType.Warning);
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Style.settingsIcon, Style.smallIconStyle, Style.smallIconOptions);

            GUILayout.Space(10);

            settingsSectionToggle = EditorGUILayout.Foldout(settingsSectionToggle, "Settings", Style.headerToggleStyle);
            GUILayout.EndHorizontal();
            //draw all settings fields
            if (settingsSectionToggle)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(terrainProperty, terrainProperty.isExpanded);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(layerProperty, layerProperty.isExpanded);

                gridSize = Mathf.Max(EditorGUILayout.FloatField("Grid Size", gridSize), 0);
                brushRadius = EditorGUILayout.Slider("Brush Size", brushRadius, 0, 100);
                brushDensity = EditorGUILayout.Slider("Brush Density", brushDensity, 0, 1);
                brushColor = EditorGUILayout.ColorField("Brush Color", brushColor);
            }

            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Editor re-draw method.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            DrawButtonsStrip();
            DrawToolSection();
            DrawSettingsSection();
            serializedObject.ApplyModifiedProperties();
        }


        #region Creation methods

        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, params BrushPrefab[] objects)
        {
            PlaceObjectsInBrush(center, gridSize, radius, density, ~0, null, objects);
        }

        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, LayerMask layer, params BrushPrefab[] objects)
        {
            PlaceObjectsInBrush(center, gridSize, radius, density, layer, null, objects);
        }

        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, LayerMask layer, Transform parent, params BrushPrefab[] objects)
        {
            if (objects == null || objects.Length == 0)
            {
                Debug.LogWarning("No objects to instantiate.");
                return;
            }

            //list of all created positions
            var grid = new List<Vector2>();
            //count calculation using circle area, provided density and single cell size
            var count = Mathf.Max((Mathf.PI * radius * radius * density) / (gridSize * 2), 1);

            for (var i = 0; i < count; i++)
            {
                //setting random properties 
                var prefab = objects[Random.Range(0, objects.Length)];
                var radians = Random.Range(0, 359) * Mathf.Deg2Rad;
                var distance = Random.Range(0.0f, radius);
                //calculating position + grid cell position
                var position = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * distance + center;
                var gridPosition = new Vector2(position.x - position.x % gridSize, position.z - position.z % gridSize);
                //position validation using grid
                if (grid.Contains(gridPosition))
                {
                    continue;
                }
                else
                {
                    grid.Add(gridPosition);
                }
                //position validation using layer
                if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, layer))
                {                  
                    var gameObject = PrefabUtility.InstantiatePrefab(prefab.target, null) as GameObject;
                    //set random rotation if needed
                    if (prefab.useRandomRotation)
                    {
                        gameObject.transform.eulerAngles = new Vector3(Random.Range(prefab.minRotation.x, prefab.maxRotation.x), 
                            Random.Range(prefab.minRotation.y, prefab.maxRotation.y), Random.Range(prefab.minRotation.z, prefab.maxRotation.z));
                    }
                    //set random scale if needed
                    if (prefab.useRandomScale)
                    {
                        gameObject.transform.localScale = new Vector3(Random.Range(prefab.minScale.x, prefab.maxScale.x),
                            Random.Range(prefab.minScale.y, prefab.maxScale.y), Random.Range(prefab.minScale.z, prefab.maxScale.z));
                    }

                    gameObject.transform.position = hitInfo.point;
                    gameObject.transform.parent = parent;

                    Undo.RegisterCreatedObjectUndo(gameObject, "Created " + gameObject.name + " with brush");
                }
            }
        }

        #endregion


        /// <summary>
        /// Serialized component.
        /// </summary>
        public new TerrainTool Target => target as TerrainTool;


        #region Dialogs

        public static bool MassPlace(int count)
        {
            return UnityEditor.EditorUtility.DisplayDialog("Mass Place", "Are you sure about placing " + count + " objects?", "Yes", "Cancel");
        }

        #endregion


        /// <summary>
        /// Internal styling class.
        /// </summary>
        internal static class Style
        {
            internal static Color brushColor;

            internal static Texture treeIcon;
            internal static Texture noneIcon;
            internal static Texture brushIcon;
            internal static Texture terrainIcon;
            internal static Texture settingsIcon;

            internal static GUIStyle smallIconStyle;
            internal static GUIStyle toolToggleStyle;
            internal static GUIStyle headerToggleStyle;

            internal static GUIContent treeToggleContent;
            internal static GUIContent noneToggleContent;
            internal static GUIContent brushToggleContent;
            internal static GUIContent terrainToggleContent;

            internal static GUILayoutOption[] toggleOptions;
            internal static GUILayoutOption[] smallIconOptions;

            static Style()
            {
                brushColor = new Color(0, 0, 0, 0.4f);

                treeIcon = ToolboxAssetUtility.LoadEditorAsset<Texture>("Editor Tree Icon.png");
                noneIcon = null;//ToolboxAssetUtility.LoadEditorAsset<Texture>("Editor None Icon.png");
                brushIcon = ToolboxAssetUtility.LoadEditorAsset<Texture>("Editor Brush Icon.png");
                terrainIcon = ToolboxAssetUtility.LoadEditorAsset<Texture>("Editor Terrain Icon.png");
                settingsIcon = ToolboxAssetUtility.LoadEditorAsset<Texture>("Editor Settings Icon.png");

                smallIconStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter
                };
                toolToggleStyle = new GUIStyle(GUI.skin.button);
                {

                };
                headerToggleStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };

                treeToggleContent = new GUIContent(treeIcon, "");
                noneToggleContent = new GUIContent(noneIcon, "Not implemented");
                brushToggleContent = new GUIContent(brushIcon, "Activate tool");
                terrainToggleContent = new GUIContent(terrainIcon, "Not implemented");

                toggleOptions = new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(40),
                    GUILayout.MaxHeight(40)
                };
                smallIconOptions = new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(20),
                    GUILayout.MaxHeight(20)
                };
            }
        }
    }
}