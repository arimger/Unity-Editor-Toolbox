using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    public abstract class ToolboxCollectionDrawer<T> : ToolboxPropertyDrawerBase where T : ToolboxCollectionAttribute
    {
        protected void DrawDefaultCollection(SerializedProperty collection)
        {
            if (!collection.isArray)
            {
                Debug.LogError(GetType() + " - cannot draw " + collection.propertyPath + " in " + collection.serializedObject + " since this is not array property.");
                return;
            }

            if (!(collection.isExpanded =
                EditorGUILayout.Foldout(collection.isExpanded, new GUIContent(collection.displayName), true)))
            {
                return;
            }

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


        public override sealed void OnGui(SerializedProperty collection)
        {
            var targetAttribute = collection.GetAttribute<T>();
            if (targetAttribute != null)
            {
                OnGui(collection, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            DrawDefaultCollection(collection);
        }

        public override sealed void OnGui(SerializedProperty collection, ToolboxAttribute attribute)
        {
            if (attribute is T targetAttribute)
            {
                OnGui(collection, targetAttribute);
                return;
            }
            else
            {
                Debug.LogError("Target attribute not found.");
            }

            DrawDefaultCollection(collection);
        }


        public virtual void OnGui(SerializedProperty collection, T attribute)
        {
            DrawDefaultCollection(collection);
        }
    }
}