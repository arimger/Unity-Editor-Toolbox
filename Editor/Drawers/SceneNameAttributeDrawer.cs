using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameAttributeDrawer : ToolboxNativePropertyDrawer
    {
        private static bool SceneExists(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;

            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/");
                var name = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0) return true;
            }

            return false;
        }


        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            const string warningMessage = "Scene does not exist. Check available Scenes in Build options.";

            if (!SceneExists(property.stringValue))
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.height);
                EditorGUI.HelpBox(helpBoxRect, warningMessage, MessageType.Warning);

                //adjust property label height
                position.height -= Style.height + Style.spacing * 2;
                //adjust OY position for target property
                position.y += Style.height + Style.spacing * 2;
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.String;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!SceneExists(property.stringValue))
            {         
                //set additional height as help box height + 2x spacing between properties
                return base.GetPropertyHeight(property, label) + Style.height + Style.spacing * 2;
            }

            return base.GetPropertyHeight(property, label);
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f * 2;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;
        }
    }
}