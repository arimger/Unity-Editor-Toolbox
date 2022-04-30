using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReferencePickerAttributeDrawer : ToolboxSelfPropertyDrawer<ReferencePickerAttribute>
    {
        private static readonly TypeField typeField = new TypeField(true, true, false, false);


        private void DrawTypeProperty(SerializedProperty property)
        {
            property.GetFieldInfo(out Type propertyType);
            //TODO: handle null
            if (!TypeUtilities.TryGetTypeFromManagedReferenceFullTypeName(property.managedReferenceFullTypename, out var currentType))
            {

            }

            var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            typeField.OnGui(position, propertyType, currentType, (type) =>
            {
                try
                {
                    //TODO: handle multiple objects
                    property.serializedObject.Update();
                    var obj = type != null ? Activator.CreateInstance(type) : null;
                    property.managedReferenceValue = obj;
                    property.serializedObject.ApplyModifiedProperties();
                }
                catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
                {
                    ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
                }
            });
        }


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReferencePickerAttribute attribute)
        {
            using (var propertyScope = new PropertyScope(property, label))
            {
                if (!propertyScope.IsVisible)
                {
                    return;
                }

                EditorGUI.indentLevel++;
                DrawTypeProperty(property);
                ToolboxEditorGui.DrawPropertyChildren(property);
                EditorGUI.indentLevel--;
            }
        }

        public override bool IsPropertyValid(SerializedProperty property)
        {
            //TODO: validate property (check for the SerializeReferenceAttribute)
            return true;
        }
    }
}