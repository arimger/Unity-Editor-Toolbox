#if UNITY_2020_1_OR_NEWER
using System;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class SerializedDictionaryDrawer : ToolboxTargetTypeDrawer
    {
        static SerializedDictionaryDrawer()
        {
            storage = new PropertyDataStorage<ReorderableListBase, CreationArgs>(false, (p, a) =>
            {
                var pairsProperty = a.pairsProperty;
                var errorProperty = a.errorProperty;

                var list = new ToolboxEditorList(pairsProperty, "Pair", true, true, false);
                list.drawHeaderCallback += (rect) =>
                {
                    //cache preprocessed label to get prefab related functions
                    var label = EditorGUI.BeginProperty(rect, null, p);
                    //create additional warning message if there is a collision
                    if (errorProperty.boolValue)
                    {
                        label.image = EditorGuiUtility.GetHelpIcon(MessageType.Warning);
                        label.text += string.Format(" [{0}]", Style.warningMessage);
                    }
                    EditorGUI.LabelField(rect, label);
                    EditorGUI.EndProperty();
                };
                list.drawFooterCallback += (rect) =>
                {
                    list.DrawStandardFooter(rect);
                };
                list.drawElementCallback += (rect, index, isActive, isFocused) =>
                {
                    var element = pairsProperty.GetArrayElementAtIndex(index);
                    var content = list.GetElementContent(element, index);

                    using (var propertyScope = new PropertyScope(element, content))
                    {
                        if (!propertyScope.IsVisible)
                        {
                            return;
                        }

                        //draw key/value children and use new, shortened labels
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("key"), new GUIContent("K"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("value"), new GUIContent("V"));
                        EditorGUI.indentLevel--;
                    }
                };
                return list;
            });
        }

        private static readonly PropertyDataStorage<ReorderableListBase, CreationArgs> storage;


        public override void OnGui(SerializedProperty property, GUIContent label)
        {
            var pairsProperty = property.FindPropertyRelative("pairs");
            var errorProperty = property.FindPropertyRelative("error");
            var drawerArgs = new CreationArgs()
            {
                pairsProperty = pairsProperty,
                errorProperty = errorProperty
            };

            storage.ReturnItem(property, drawerArgs).DoList();
        }

        public override Type GetTargetType()
        {
            return typeof(SerializedDictionary<,>);
        }

        public override bool UseForChildren()
        {
            return false;
        }


        private static class Style
        {
            internal static readonly string warningMessage = "keys are not unique, it will break deserialization";
        }

        private struct CreationArgs
        {
            public SerializedProperty pairsProperty;
            public SerializedProperty errorProperty;
        }
    }
}
#endif