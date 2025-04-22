using Toolbox.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneDetailsAttribute))]
    [CustomPropertyDrawer(typeof(SerializedScene))]
    public class SerializedSceneDrawer : PropertyDrawerBase
    {
        private bool HasSceneDetails(SerializedProperty property)
        {
            var sceneProperty = property.FindPropertyRelative("sceneReference");
            return attribute != null && attribute is SceneDetailsAttribute && sceneProperty.objectReferenceValue;
        }

        private void DrawSceneDetails(Rect position, SceneData sceneData)
        {
            EditorGUI.BeginDisabledGroup(true);
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.IntField(position, Style.buildIndexContent, sceneData.index);
            position.y += EditorGUIUtility.singleLineHeight + spacing;
            EditorGUI.Toggle(position, Style.isEnabledContent, sceneData.enabled);
            EditorGUI.EndDisabledGroup();
        }

        private void OpenBuildSettings()
        {
            EditorWindow.GetWindow(typeof(BuildPlayerWindow));
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var hasDetails = HasSceneDetails(property);
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            if (hasDetails)
            {
                position.xMax -= Style.foldoutWidth;
            }

            var sceneProperty = property.FindPropertyRelative("sceneReference");
            EditorGUI.ObjectField(position, sceneProperty, label);
            EditorGUI.EndProperty();

            if (hasDetails)
            {
                var prevXMin = position.xMin;
                position.xMin = position.xMax;
                position.xMax += Style.foldoutWidth;
                using (new DisabledScope(true))
                {
                    property.isExpanded = GUI.Toggle(position, property.isExpanded, Style.foldoutContent, Style.foldoutStyle);
                }

                position.xMin = prevXMin;
            }

            if (!hasDetails || !property.isExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;
            var sceneData = SceneData.GetSceneDataFromIndex(property);
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            position.y += EditorGUIUtility.singleLineHeight + spacing;
            if (sceneData.inBuild)
            {
                DrawSceneDetails(position, sceneData);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.LabelField(position, Style.notInBuildContent);
                position.y += EditorGUIUtility.singleLineHeight + spacing;
                EditorGUI.EndDisabledGroup();
                var buttonRect = EditorGUI.IndentedRect(position);
                if (GUI.Button(buttonRect, Style.showDetailsContent))
                {
                    OpenBuildSettings();
                }
            }

            EditorGUI.indentLevel--;
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedScene);
        }

        private struct SceneData
        {
            public int index;
            public bool enabled;
            public bool inBuild;

            public static SceneData GetSceneDataFromIndex(SerializedProperty property)
            {
                var indexProperty = property.FindPropertyRelative("buildIndex");
                var index = indexProperty.intValue;
                return new SceneData()
                {
                    index = index,
                    enabled = index != -1,
                    inBuild = index != -1
                };
            }

            public static SceneData GetSceneDataFromScene(SerializedProperty property)
            {
                var sceneData = new SceneData()
                {
                    index = -1,
                    enabled = false,
                    inBuild = false
                };

                var sceneAsset = property.objectReferenceValue as SceneAsset;
                var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                var sceneGuid = AssetDatabase.AssetPathToGUID(scenePath);
                var sceneIndex = -1;
                for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    var sceneSettings = EditorBuildSettings.scenes[i];
                    var isEnabled = sceneSettings.enabled && !string.IsNullOrEmpty(sceneSettings.path);
                    if (isEnabled)
                    {
                        sceneIndex++;
                    }

                    var guid = sceneSettings.guid;
                    if (guid.Equals(new GUID(sceneGuid)))
                    {
                        sceneData.index = isEnabled ? sceneIndex : -1;
                        sceneData.enabled = isEnabled;
                        sceneData.inBuild = true;
                        break;
                    }
                }

                return sceneData;
            }
        }

        private static class Style
        {
            internal const float foldoutWidth = 50.0f;

            internal static readonly GUIContent foldoutContent = new GUIContent("Details", "Show/Hide Scene Details");
            internal static readonly GUIContent buildIndexContent = new GUIContent("Build Index");
            internal static readonly GUIContent isEnabledContent = new GUIContent("Is Enabled");
            internal static readonly GUIContent notInBuildContent = new GUIContent("Not in Build");
            internal static readonly GUIContent showDetailsContent = new GUIContent("Open Build Settings");

            internal static readonly GUIStyle foldoutStyle;

            static Style()
            {
                foldoutStyle = new GUIStyle(EditorStyles.miniButton)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 10,
#else
                    fontSize = 9,
#endif
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }
    }
}