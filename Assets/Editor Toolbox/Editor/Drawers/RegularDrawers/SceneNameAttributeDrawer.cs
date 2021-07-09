using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private static bool SceneExists(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/");
                var name = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return SceneExists(property.stringValue)
                ? base.GetPropertyHeightSafe(property, label)
                : base.GetPropertyHeightSafe(property, label) + Style.boxHeight + Style.spacing * 2;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!SceneExists(property.stringValue))
            {
                var helpBoxRect = new Rect(position.x,
                                           position.y,
                                           position.width, Style.boxHeight);
                EditorGUI.HelpBox(helpBoxRect, "Scene does not exist. " +
                                               "Check available Scenes in the Build options.", MessageType.Warning);
                position.yMin += Style.boxHeight + Style.spacing * 2;
            }

            position.height = Style.rowHeight;
            position.xMax -= Style.pickerWidth + Style.spacing;
            EditorGUI.PropertyField(position, property, label);
            position.xMin += position.width;
            position.xMax += Style.pickerWidth + Style.spacing;

            var controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            if (GUI.Button(position, Style.pickerButtonContent, EditorStyles.miniButton))
            {
                EditorGUIUtility.ShowObjectPicker<SceneAsset>(null, false, null, controlId);
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                if (controlId == EditorGUIUtility.GetObjectPickerControlID())
                {
                    var target = EditorGUIUtility.GetObjectPickerObject();
                    if (target)
                    {
                        property.serializedObject.Update();
                        property.stringValue = target.name;
                        property.serializedObject.ApplyModifiedProperties();
                        GUI.changed = true;
                    }
                }
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }


        private static class Style
        {
            internal static readonly float rowHeight = EditorGUIUtility.singleLineHeight;
#if UNITY_2019_3_OR_NEWER
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
#else
            internal static readonly float boxHeight = EditorGUIUtility.singleLineHeight * 2.5f;
#endif
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float pickerWidth = 30.0f;

            internal static readonly GUIContent pickerButtonContent;

            static Style()
            {
                pickerButtonContent = new GUIContent(EditorGUIUtility.FindTexture("UnityLogoLarge"), "Pick SceneAsset");
            }
        }
    }
}