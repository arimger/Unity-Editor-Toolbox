using System;
using System.Collections.Generic;

using UnityEngine;

namespace Toolbox.Editor.Internal
{
    public class TypeField
    {
        private bool allowAbstract;
        private bool allowObsolete;
        private bool addSearchField;


        public void OnGui(Rect position, Type parentType)
        {
            //TODO:
            //ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
            //{
            //    try
            //    {
            //        //TODO: handle multiple objects
            //        property.serializedObject.Update();
            //        var type = i >= 0 ? filteredTypes[i] : null;
            //        var obj = type != null ? Activator.CreateInstance(type) : null;
            //        property.managedReferenceValue = obj;
            //        property.serializedObject.ApplyModifiedProperties();
            //    }
            //    catch (Exception e) when (e is ArgumentNullException || e is NullReferenceException)
            //    {
            //        ToolboxEditorLog.LogWarning("Invalid attempt to update disposed property.");
            //    }
            //});
        }


        public IReadOnlyList<Type> CachedTypes { get; private set; }
    }
}