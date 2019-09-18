using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameAttributeDrawer : PropertyDrawer
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


        private float additionalHeight;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + additionalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                Debug.LogWarning(property.name + " property in " + property.serializedObject.targetObject +
                                 " - " + attribute.GetType() + " can be used only on string value properties.");
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            additionalHeight = 0;

            if (!SceneExists(property.stringValue))
            {
                var helpBoxRect = new Rect(position.x, position.y, position.width, Style.height);
                EditorGUI.HelpBox(helpBoxRect, "Scene does not exist. Check available Scenes in Build options.", MessageType.Warning);
                //set additional height as help box height + 2x spacing between properties
                additionalHeight = Style.height + Style.spacing * 2;
                position.height -= additionalHeight;
                //adjust OY position for target property
                position.y += additionalHeight;
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }


        private static class Style
        {
            internal static readonly float height = EditorGUIUtility.singleLineHeight * 1.25f * 2;
            internal static readonly float spacing = EditorGUIUtility.standardVerticalSpacing;
            internal static readonly float padding = 5.0f;
        }
    }
}