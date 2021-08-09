using System;
using System.Reflection;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    public class ReorderableListExposedAttributeDrawer : ToolboxListPropertyDrawer<ReorderableListExposedAttribute>
    {
        static ReorderableListExposedAttributeDrawer()
        {
            storage = new PropertyDataStorage<ReorderableListBase, ReorderableListExposedAttribute>(false, (p, a) =>
            {
                //create list in the standard way
                var list = ToolboxEditorGui.CreateList(p,
                    a.ListStyle,
                    a.ElementLabel,
                    a.FixedSize,
                    a.Draggable,
                    a.HasHeader,
                    a.HasLabels,
                    a.Foldable);
                //additionaly subscribe callbacks
                ConnectCallbacks(list, a);
                return list;
            });
        }

        private static readonly PropertyDataStorage<ReorderableListBase, ReorderableListExposedAttribute> storage;


        private static void ConnectCallbacks(ReorderableListBase list, ReorderableListExposedAttribute attribute)
        {
            var listTarget = list.SerializedObject;
            var fieldInfo = list.List.GetFieldInfo();
            var returnType = fieldInfo.FieldType.GetEnumeratedType();
            var methodName = attribute.OverrideNewElementMethodName;
            var methodInfo = FindMethod(listTarget, methodName, returnType);
            if (methodInfo == null)
            {
                return;
            }

            list.overrideNewElementCallback = (index) =>
            {
                return methodInfo.Invoke(listTarget.targetObject, null);
            };
        }

        private static MethodInfo FindMethod(SerializedObject target, string methodName, Type expectedReturnType = null)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return null;
            }

            var methodInfo = ReflectionUtility.GetObjectMethod(methodName, target);
            if (methodInfo == null)
            {
                ToolboxEditorLog.AttributeUsageWarning(typeof(ReorderableListExposedAttribute), string.Format("{0} method not found.", methodName));
                return null;
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                ToolboxEditorLog.AttributeUsageWarning(typeof(ReorderableListExposedAttribute), string.Format("{0} method not found.", methodName));
                return null;
            }

            if (expectedReturnType != null && expectedReturnType != methodInfo.ReturnType)
            {
                ToolboxEditorLog.AttributeUsageWarning(typeof(ReorderableListExposedAttribute), string.Format("{0} method returns invalid type. Expected - {1}.", methodName, expectedReturnType));
                return null;
            }

            return methodInfo;
        }


        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, ReorderableListExposedAttribute attribute)
        {
            storage.ReturnItem(property, attribute).DoList();
        }
    }
}