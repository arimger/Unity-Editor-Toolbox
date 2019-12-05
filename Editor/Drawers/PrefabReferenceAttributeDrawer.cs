using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PrefabReferenceAttribute))]
    public class PrefabReferenceAttributeDrawer : ToolboxNativeDrawer
    {
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property);

            if (!EditorGUI.EndChangeCheck() || property.objectReferenceValue == null)
            {
                return;
            }

            if (PrefabUtility.GetPrefabAssetType(property.objectReferenceValue) == PrefabAssetType.NotAPrefab)
            {
                property.objectReferenceValue = null;
                Debug.LogWarning("Assigned Object must be a prefab.");
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}
