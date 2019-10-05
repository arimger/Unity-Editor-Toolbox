using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxPropertyDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxPropertyAttribute
    {
        /// <summary>
        /// Attribute type associated with this drawer.
        /// </summary>
        public static System.Type GetAttributeType() => typeof(T);


        /// <summary>
        /// Generate new key based on <see cref="SerializedProperty"/> hash code.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected static string GenerateKey(SerializedProperty property)
        {
            return property.serializedObject.GetHashCode() + "-" + property.name;
        }


        public override sealed void OnGui(SerializedProperty property)
        {
            var targetAttribute = property.GetAttribute<T>();
            if (targetAttribute != null)
            {
                OnGui(property, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property);
        }

        public override sealed void OnGui(SerializedProperty property, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGui(property, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            base.OnGui(property, attribute);
        }

        public virtual void OnGui(SerializedProperty property, T attribute)
        {
            base.OnGui(property);
        }
    }
}