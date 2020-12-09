using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializedScene))]
    public class SerializedSceneDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            var sceneProperty = property.FindPropertyRelative("sceneReference");
            EditorGUI.ObjectField(position, sceneProperty, GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}