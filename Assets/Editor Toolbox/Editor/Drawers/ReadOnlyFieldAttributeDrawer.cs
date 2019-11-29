using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    /// <summary>
    /// Creates field inside disabled group. If possible - use <see cref="DisableAttribute"/>,
    /// <see cref="ToolboxConditionAttribute"/>s are more flexible and can work with any additional drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public class ReadOnlyFieldDrawer : ToolboxNativeDrawerBase
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}