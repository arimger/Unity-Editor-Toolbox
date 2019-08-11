using System;

using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Tools
{
    public sealed class ConstructionEditorWindow : ToolEditorWindow
    {
        /// <summary>
        /// Tool initialization.
        /// </summary>
        [MenuItem("Window/Tools/Construction Editor")]
        public static void Init()
        {
            GetWindow<ConstructionEditorWindow>("Construction Creator").Show();
        }


        private const string defaultPath = "";

        #region Fields

        private bool constructionSquareShown = true;
        private bool constructionSettingsShown = true;

        private bool[] selectedSquares = new bool[3 * 3];

        private string constructionName;
        private string constructionsPath = defaultPath;

        private GameObject constructionMesh;

        private Vector2Int constructionSize = new Vector2Int(3, 3);

        private Vector2 constructionPivot = new Vector2(0.5f, 0.5f);

        #endregion

        private void SetAllSquares(bool value)
        {
            for (var i = 0; i < selectedSquares.Length; i++)
            {
                selectedSquares[i] = value;
            }
        }


        private void BuildRoads()
        {
            throw new NotImplementedException();
        }

        private void BuildConstruction()
        {
            throw new NotImplementedException();
        }

        private void DrawConstructionSquare()
        {
            EditorGUILayout.BeginVertical(Style.sectionStyle);
            if (constructionSquareShown = EditorGUILayout.Foldout(constructionSquareShown, "Construction Site", Style.sectionHeaderStyle))
            {
                EditorGUILayout.Space();
                for (var i = 0; i < constructionSize.x; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    for (var j = 0; j < constructionSize.y; j++)
                    {
                        var index = i * constructionSize.y + j;
                        EditorGUILayout.BeginHorizontal(Style.sectionStyle);
                        selectedSquares[index] = GUILayout.Toggle(selectedSquares[index], i + "x" + j, Style.squareButtonOptions);
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select all", Style.sectionButtonOptions))
                {
                    SetAllSquares(true);
                }
                if (GUILayout.Button("Reset", Style.sectionButtonOptions))
                {
                    SetAllSquares(false);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawConstructionSettings()
        {
            EditorGUILayout.BeginVertical(Style.sectionStyle);
            if (constructionSettingsShown = EditorGUILayout.Foldout(constructionSettingsShown, "Construction Settings", Style.sectionHeaderStyle))
            {
                EditorGUILayout.Space();
                constructionsPath = EditorGUILayout.TextField("Prefab Save Path", constructionsPath);
                constructionName = EditorGUILayout.TextField("Construction Name", constructionName);
                constructionMesh = EditorGUILayout.ObjectField("Construction Mesh", constructionMesh, typeof(GameObject), true) as GameObject;
                ToolboxEditorUtility.DrawLayoutAssetPreview(constructionMesh, 90, 90);
                constructionPivot = EditorGUILayout.Vector2Field("Construction Mesh Pivot", constructionPivot);
                constructionSize = EditorGUILayout.Vector2IntField("Construction Size", constructionSize);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            if (selectedSquares.Length != constructionSize.x * constructionSize.y)
            {
                Array.Resize(ref selectedSquares, constructionSize.x * constructionSize.y);
            }
        }

        private void DrawWindowFooter()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("      Create      "))
            {
                BuildConstruction();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }


        /// <summary>
        /// Tool initialization.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// Editor re-draw method.
        /// </summary>
        protected override void OnGUI()
        {
            base.OnGUI();

            DrawConstructionSettings();
            DrawConstructionSquare();

            EditorGUILayout.HelpBox("Functionalities not implemented yet.", MessageType.Warning);

            DrawWindowFooter();
        }


        /// <summary>
        /// Internal styling class.
        /// </summary>
        internal static class Style
        {
            internal const float padding = 5.0f;

            internal static GUIStyle sectionStyle;
            internal static GUIStyle smallIconStyle;
            internal static GUIStyle sectionHeaderStyle;

            internal static GUILayoutOption[] toggleOptions;
            internal static GUILayoutOption[] smallIconOptions;
            internal static GUILayoutOption[] squareButtonOptions;
            internal static GUILayoutOption[] sectionButtonOptions;

            static Style()
            {
                sectionStyle = new GUIStyle(GUI.skin.box);

                smallIconStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter
                };
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout)
                {                
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = FontStyle.Bold,
                    contentOffset = new Vector2(padding, 0)
                };

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
                squareButtonOptions = new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(40),
                    GUILayout.MaxHeight(40),
                    GUILayout.MinWidth(20),
                    GUILayout.MinHeight(20)
                };
                sectionButtonOptions = new GUILayoutOption[]
                {
                    GUILayout.MaxWidth(65)
                };
            }
        }
    }
}