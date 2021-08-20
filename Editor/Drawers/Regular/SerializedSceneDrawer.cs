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
            return attribute != null && sceneProperty.objectReferenceValue;
        }

        private void DrawIncludedSceneDetails(Rect position, SceneData sceneData)
        {
            EditorGUI.BeginDisabledGroup(true);
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.IntField(position, Style.buildIndexContent, sceneData.index);
            position.y += EditorGUIUtility.singleLineHeight + spacing;
            EditorGUI.Toggle(position, Style.isEnabledContent, sceneData.enabled);
            EditorGUI.EndDisabledGroup();
        }

        private void DrawRejectedSceneDetails(Rect position, SceneData sceneData)
        {
            EditorGUI.BeginDisabledGroup(true);
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(position, Style.notInBuildContent);
            position.y += EditorGUIUtility.singleLineHeight + spacing;
            EditorGUI.EndDisabledGroup();
            if (GUI.Button(position, Style.showDetailsContent))
            {
                OpenBuildSettings();
            }
        }

        private void OpenBuildSettings()
        {
            EditorWindow.GetWindow(typeof(BuildPlayerWindow));
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return HasSceneDetails(property)
                ? base.GetPropertyHeightSafe(property, label) + lineHeight * 2
                : base.GetPropertyHeightSafe(property, label);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            position = EditorGUI.PrefixLabel(position, label);
            var sceneProperty = property.FindPropertyRelative("sceneReference");
            EditorGUI.ObjectField(position, sceneProperty, GUIContent.none);
            EditorGUI.EndProperty();

            if (!HasSceneDetails(property))
            {
                return;
            }

            var sceneData = SceneData.GetSceneData(sceneProperty);
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            position.y += EditorGUIUtility.singleLineHeight + spacing;
            if (sceneData.inBuild)
            {
                DrawIncludedSceneDetails(position, sceneData);
            }
            else
            {
                DrawRejectedSceneDetails(position, sceneData);
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.type == nameof(SerializedScene);
        }


        private struct SceneData
        {
            public int index;
            public bool enabled;
            public GUID guid;
            public bool inBuild;

            public static SceneData GetSceneData(SerializedProperty property)
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
                for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    var sceneSettings = EditorBuildSettings.scenes[i];
                    var guid = sceneSettings.guid;
                    if (guid.Equals(new GUID(sceneGuid)))
                    {
                        sceneData.index = i;
                        sceneData.enabled = sceneSettings.enabled;
                        sceneData.guid = guid;
                        sceneData.inBuild = true;
                    }
                }

                return sceneData;
            }
        }

        private static class Style
        {
            internal static readonly GUIContent buildIndexContent = new GUIContent("Build Index");
            internal static readonly GUIContent isEnabledContent = new GUIContent("Is Enabled");
            internal static readonly GUIContent notInBuildContent = new GUIContent("Not in Build");
            internal static readonly GUIContent showDetailsContent = new GUIContent("Open Build Settings");
        }
    }
}