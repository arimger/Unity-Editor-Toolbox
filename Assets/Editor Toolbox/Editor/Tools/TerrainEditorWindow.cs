using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using UnityTools = UnityEditor.Tools;

namespace Toolbox.Editor.Tools
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Editor for <see cref="TerrainTool"/> component. Provides tools to manipulate game environment.
    /// </summary>
    [CustomEditor(typeof(TerrainTool), true, isFallback = false)]
    public class TerrainEditorWindow : ComponentToolEditor
    {
        private bool settingsToggle = true;
        private bool isPaintingTrees;
        private bool isPaintingBushes;

        /// <summary>
        /// If any provided tool is currently active returns true.
        /// </summary>
        private bool IsAnyToolSelected
        {
            get => isPaintingTrees || isPaintingBushes;
        }

        private float gridSize = 1.0f;
        private float brushRadius = 8.0f;
        private float brushDensity = 0.5f;

        private Color brushColor = new Color(0, 0, 0, 0.4f);

        private EditorPaintToolSection treeSection;
        private EditorPaintToolSection bushSection;

        private SerializedProperty layerProperty;
        private SerializedProperty terrainProperty;
        private SerializedProperty treePrefabsProperty;
        private SerializedProperty bushPrefabsProperty;

        private ReorderableList treePrefabsList;
        private ReorderableList bushPrefabsList;


        /// <summary>
        /// Tool initialization.
        /// </summary>
        protected override void OnEnable()
        {
            gridSize = EditorPrefs.GetFloat("TerrainEditor.gridSize", 1.0f);
            brushRadius = EditorPrefs.GetFloat("TerrainEditor.brushRadius", 8.0f);
            brushDensity = EditorPrefs.GetFloat("TerrainEditor.brushDensity", 50.0f);

            base.OnEnable();

            layerProperty = serializedObject.FindProperty("terrainLayer");
            terrainProperty = serializedObject.FindProperty("terrain");
            treePrefabsProperty = serializedObject.FindProperty("treePrefabs");
            bushPrefabsProperty = serializedObject.FindProperty("bushPrefabs");

            treePrefabsList = ToolboxEditorUtility.CreateLinedList(treePrefabsProperty, "Item");
            bushPrefabsList = ToolboxEditorUtility.CreateLinedList(bushPrefabsProperty, "Item");

            var treeIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Tree Icon.png");
            var bushIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Bush Icon.png"); 

            treeSection = new EditorPaintToolSection("Tree", treeIcon, treePrefabsList);
            bushSection = new EditorPaintToolSection("Bush", bushIcon, bushPrefabsList);
        }

        /// <summary>
        /// Tool deinitialization.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            EditorPrefs.SetFloat("TerrainEditor.gridSize", gridSize);
            EditorPrefs.SetFloat("TerrainEditor.brushRadius", brushRadius);
            EditorPrefs.SetFloat("TerrainEditor.brushDensity", brushDensity);
        }

        /// <summary>
        /// Scene view managment used in tool functionality.
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            if (!IsAnyToolSelected)
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
                        if (isPaintingTrees)
                        {
                            PlaceObjectsInBrush(hit.point, gridSize, brushRadius, brushDensity, 
                                Target.TerrainLayer, Target.Terrain.transform, Target.Trees);
                        }
                        if (isPaintingBushes)
                        {
                            PlaceObjectsInBrush(hit.point, gridSize, brushRadius, brushDensity, 
                                Target.TerrainLayer, Target.Terrain.transform, Target.Bushes);
                        }
                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    } 
                }
            }

            SceneView.RepaintAll();
        }

        /// <summary>
        /// Editor re-draw method.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            DrawButtonsStrip();
            DrawToolSections();
            DrawSettingsSection();
            serializedObject.ApplyModifiedProperties();
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
            isPaintingTrees = GUILayout.Toggle(isPaintingTrees, Style.treeToggleContent, GUI.skin.button, Style.toggleOptions);
            isPaintingBushes = GUILayout.Toggle(isPaintingBushes, Style.bushToggleContent, GUI.skin.button, Style.toggleOptions);
            //TODO: environment tool;
            GUILayout.Toggle(false, "", GUI.skin.button, Style.toggleOptions);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (IsAnyToolSelected)
            {
                EditorGUILayout.HelpBox("Tool active \n\n" +
                                        "Navigate mouse to desired terrain and press left button to create objects \n\n" +
                                        "Ctrl+Z - Undo", MessageType.Info);
            }
        }

        /// <summary>
        /// Draws all tool sections using previously creates <see cref="EditorPaintToolSection"/> objects.
        /// </summary>
        private void DrawToolSections()
        {
            if (isPaintingTrees) treeSection.DoLayout();
            if (isPaintingBushes) bushSection.DoLayout();
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

            settingsToggle = EditorGUILayout.Foldout(settingsToggle, "Settings", Style.headerToggleStyle);
            GUILayout.EndHorizontal();
            //all settings fields
            if (settingsToggle)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(terrainProperty, terrainProperty.isExpanded);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(layerProperty, layerProperty.isExpanded);

                gridSize = Mathf.Max(EditorGUILayout.FloatField("Grid Size", gridSize), 0);
                brushRadius = EditorGUILayout.Slider("Brush Size", brushRadius, 0, 100);
                brushDensity = EditorGUILayout.Slider("Density", brushDensity, 0, 1);
            }
            EditorGUILayout.EndVertical();
        }


        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, params GameObject[] objects)
        {
            PlaceObjectsInBrush(center, gridSize, radius, density, ~0, null, objects);
        }

        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, LayerMask layer, params GameObject[] objects)
        {
            PlaceObjectsInBrush(center, gridSize, radius, density, layer, null, objects);
        }

        private void PlaceObjectsInBrush(Vector3 center, float gridSize, float radius, float density, LayerMask layer, Transform parent, params GameObject[] objects)
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
                var rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
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
                    var gameObject = PrefabUtility.InstantiatePrefab(prefab, null) as GameObject;
                    gameObject.transform.position = hitInfo.point;
                    gameObject.transform.rotation = rotation;

                    Undo.RegisterCreatedObjectUndo(gameObject, "Created " + gameObject.name + " with brush");
                }
            }
        }


        /// <summary>
        /// Serialized component.
        /// </summary>
        public new TerrainTool Target => target as TerrainTool;


        /// <summary>
        /// Wrapper class which allows to draw custom tool section.
        /// </summary>
        public class EditorPaintToolSection
        {
            private readonly ReorderableList prefabsList;


            public EditorPaintToolSection(string toolName, Texture tooIcon, SerializedProperty prefabs)
                : this(toolName, tooIcon, ToolboxEditorUtility.CreateRoundList(prefabs)) { }

            public EditorPaintToolSection(string toolName, Texture toolIcon, ReorderableList prefabsList)
            {
                IsOn = true;

                ToolName = toolName;
                ToolIcon = toolIcon;

                this.prefabsList = prefabsList;
            }


            /// <summary>
            /// Draw tool section using <see cref="GUILayout"/> classes.
            /// </summary>
            public void DoLayout()
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                if (ToolIcon)
                {
                    GUILayout.Label(ToolIcon, Style.smallIconStyle, Style.smallIconOptions);
                    GUILayout.Space(10);
                }
                IsOn = EditorGUILayout.Foldout(IsOn, ToolName + " Options", Style.headerToggleStyle);
                GUILayout.EndHorizontal();

                if (IsOn)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space(); 
                    
                    prefabsList.DoLayoutList();

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


            public bool IsOn { get; private set; }

            public string ToolName { get; set; }

            public Texture ToolIcon { get; set; }
        }
  

        #region Dialogs

        public static bool MassPlace(int count)
        {
            return EditorUtility.DisplayDialog("Mass Place", "Are you sure about placing " + count + " objects?", "Yes", "Cancel");
        }

        #endregion

        /// <summary>
        /// Internal styling class.
        /// </summary>
        internal static class Style
        {
            internal static Color brushColor;

            internal static Texture treeIcon;
            internal static Texture bushIcon;
            internal static Texture settingsIcon;

            internal static GUIStyle smallIconStyle;
            internal static GUIStyle headerToggleStyle;

            internal static GUIContent treeToggleContent;
            internal static GUIContent bushToggleContent;

            internal static GUILayoutOption[] toggleOptions;
            internal static GUILayoutOption[] smallIconOptions;

            static Style()
            {
                brushColor = new Color(0, 0, 0, 0.4f);

                treeIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Tree Icon.png");
                bushIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Bush Icon.png");
                settingsIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Settings Icon.png");

                smallIconStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter
                };
                headerToggleStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };

                treeToggleContent = new GUIContent(treeIcon, "Paint trees.");
                bushToggleContent = new GUIContent(bushIcon, "Paint bushes.");

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