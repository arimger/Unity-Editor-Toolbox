using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxNativeDrawerBase : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsPropertyValid(property))
            {
                throw new WrongAttributeUsageException(property, attribute);
            }
        }


        protected virtual bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }
    }
}
