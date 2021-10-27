using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    public static class DraggingUtility
    {
        static DraggingUtility()
        {
            validateAssignmentMethod = typeof(EditorGUI)
                .GetMethod("ValidateObjectFieldAssignment",
                BindingFlags.NonPublic | BindingFlags.Static);
            appendFoldoutValueMethod = typeof(SerializedProperty)
                .GetMethod("AppendFoldoutPPtrValue",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }


        private static readonly MethodInfo validateAssignmentMethod;
        private static readonly MethodInfo appendFoldoutValueMethod;

        private static readonly int dragAndDropHash = "customDragAndDrop".GetHashCode();


        public static Object ValidateAssignment(Object[] references, SerializedProperty property, Type type, bool exactType)
        {
#if UNITY_2017_1_OR_NEWER
            var methodParams = new object[4];
            methodParams[3] = exactType ? 1 : 0;
#else
            var methodParams = new object[3];
#endif
            methodParams[0] = references;
            methodParams[1] = type;
            methodParams[2] = property;
            return validateAssignmentMethod?.Invoke(null, methodParams) as Object;
        }

        public static void AppendDragAndDropValue(Object value, SerializedProperty property)
        {
            property.serializedObject.Update();
            appendFoldoutValueMethod?.Invoke(property, new object[] { value });
            property.serializedObject.ApplyModifiedProperties();
        }

        public static void DoDragAndDropForProperty(Rect rect, SerializedProperty property)
        {
            var controlId = GUIUtility.GetControlID(dragAndDropHash, FocusType.Passive, rect);
            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.DragExited:
                    if (GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }

                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (GUI.enabled && rect.Contains(currentEvent.mousePosition))
                    {
                        var references = DragAndDrop.objectReferences;
                        var candidates = new Object[1];
                        var dragAccepted = true;
                        foreach (var o in references)
                        {
                            candidates[0] = o;
                            var validatedObject = ValidateAssignment(candidates, property, null, false);
                            if (validatedObject)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    AppendDragAndDropValue(validatedObject, property);
                                    dragAccepted = true;
                                    DragAndDrop.activeControlID = 0;
                                }
                                else
                                {
                                    DragAndDrop.activeControlID = controlId;
                                }
                            }
                        }

                        if (dragAccepted)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                            GUIUtility.ExitGUI();
                        }
                    }

                    break;
            }
        }
    }
}