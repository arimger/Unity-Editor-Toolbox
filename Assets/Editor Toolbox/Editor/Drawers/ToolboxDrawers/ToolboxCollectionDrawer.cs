using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxCollectionDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxCollectionAttribute
    {
        protected void DrawDefaultCollection(SerializedProperty collection, GUIContent label)
        {
            if (!collection.isArray)
            {
                Debug.LogError(GetType() + " - cannot draw " + collection.propertyPath + " in " + collection.serializedObject + " since this is not array property.");
                return;
            }

            if (!EditorGUILayout.PropertyField(collection, label, false))
            {
                return;
            }

            if (GUI.changed) Event.current.Use();

            var iterProperty = collection.Copy();
            var lastProperty = iterProperty.GetEndProperty();

            EditorGUI.indentLevel++;

            if (iterProperty.NextVisible(true))
            {
                EditorGUILayout.PropertyField(iterProperty.Copy());

                while (iterProperty.NextVisible(false))
                {
                    if (SerializedProperty.EqualContents(iterProperty, lastProperty))
                    {
                        break;
                    }

                    ToolboxEditorGui.DrawLayoutToolboxProperty(iterProperty.Copy());
                }
            }

            EditorGUI.indentLevel--;
        }


        public override sealed void OnGui(SerializedProperty collection, GUIContent label)
        {
            var targetAttribute = collection.GetAttribute<T>();
            if (targetAttribute != null)
            {
                OnGui(collection, label, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            DrawDefaultCollection(collection, label);
        }

        public override sealed void OnGui(SerializedProperty collection, GUIContent label, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGui(collection, label, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            DrawDefaultCollection(collection, label);
        }


        public virtual void OnGui(SerializedProperty collection, GUIContent label, T attribute)
        {
            DrawDefaultCollection(collection, label);
        }
    }
}