using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using UnityTools = UnityEditor.Tools;

//TODO: brush color in options; environment tool;

namespace Toolbox.Editor.Tools
{
    using Toolbox.Editor.Internal;

    /// <summary>
    /// Editor for <see cref="TerrainTool"/> component. Provides tools to manipulate game environment.
    /// </summary>
    [CustomEditor(typeof(TerrainTool), true, isFallback = false)]
    public class TerrainToolEditor : ToolEditor
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
            settingsSectionToggle = EditorPrefs.GetBool("TerrainEditor.settingsSectionToggle", true);
            paintingSectionToggle = EditorPrefs.GetBool("TerrainEditor.paintingSectionToggle", true);

            gridSize = EditorPrefs.GetFloat("TerrainEditor.gridSize", 1.0f);
            brushRadius = EditorPrefs.GetFloat("TerrainEditor.brushRadius", 8.0f);
            brushDensity = EditorPrefs.GetFloat("TerrainEditor.brushDensity", 50.0f);

            base.OnEnable();

            layerProperty = serializedObject.FindProperty("terrainLayer");
            terrainProperty = serializedObject.FindProperty("terrain");
            brushPrefabsProperty = serializedObject.FindProperty("brushPrefabs");

            brushPrefabsList = ToolboxEditorUtility.CreateLinedList(brushPrefabsProperty, "Item");

            var treeIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Tree Icon.png");
            var bushIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Bush Icon.png");
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
        }

        /// <summary>
        /// Scene view managment used in tool functionality.
        /// </summary>
        protected virtual void OnSceneGUI()
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
            //all settings fields
            if (settingsSectionToggle)
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

        #region Creation methods

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

        #endregion


        /// <summary>
        /// Serialized component.
        /// </summary>
        public new TerrainTool Target => target as TerrainTool;


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

                treeIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Tree Icon.png");
                noneIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor None Icon.png");
                brushIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Brush Icon.png");
                terrainIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Terrain Icon.png");
                settingsIcon = ToolboxEditorUtility.LoadEditorAsset<Texture>("Editor Settings Icon.png");

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