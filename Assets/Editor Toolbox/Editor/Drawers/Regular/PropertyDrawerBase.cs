using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    /// <summary>
    /// Base class for Toolbox drawers based on the native <see cref="PropertyDrawer"/> implementation. 
    /// </summary>
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        /// <summary>
        /// Safe equivalent of the <see cref="GetPropertyHeight"/> method.
        /// Provided property is previously validated by the <see cref="IsPropertyValid(SerializedProperty)"/> method.
        /// </summary>
        protected virtual float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// Safe equivalent of the <see cref="OnGUI"/> method.
        /// Provided property is previously validated by the <see cref="IsPropertyValid(SerializedProperty)"/> method.
        /// </summary>
        protected virtual void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }


        /// <summary>
        /// Native call to return the expected height.
        /// </summary>
        public override sealed float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsPropertyValid(property)
                ? GetPropertyHeightSafe(property, label)
                : base.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// Native call to draw the provided property.
        /// </summary>
        public override sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsPropertyValid(property))
            {
                OnGUISafe(position, property, label);
                return;
            }

            var warningContent = new GUIContent(property.displayName + " has invalid property drawer");
            //create additional warning log to the console window
            ToolboxEditorLog.WrongAttributeUsageWarning(attribute, property);
            //create additional warning label based on the property name
            ToolboxEditorGui.DrawEmptyProperty(position, property, warningContent);
        }


        /// <summary>
        /// Checks if provided property can be properly handled by this drawer.
        /// </summary>
        public virtual bool IsPropertyValid(SerializedProperty property)
        {
            return true;
        }
    }
}