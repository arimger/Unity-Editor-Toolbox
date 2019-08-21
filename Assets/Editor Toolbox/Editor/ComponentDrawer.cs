using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    //[CustomPropertyDrawer(typeof(Toolbox.))]
    public class ComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
}