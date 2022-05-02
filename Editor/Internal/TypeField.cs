using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    //TODO: refactor
    public class TypeField
    {
        private readonly bool addEmptyValue;
        private readonly bool addSearchField;
        private readonly TypesGroupSettings groupingSettings;


        public TypeField(bool addEmptyValue = true, bool addSearchField = true, bool allowAbstract = false, bool allowObsolete = false)
            : this(addEmptyValue, addSearchField, new TypeConstraintStandard(null, TypeSettings.Class, allowAbstract, allowObsolete))
        { }

        public TypeField(bool addEmptyValue = true, bool addSearchField = true, TypeConstraint typeConstraint = null)
        {
            this.addEmptyValue = addEmptyValue;
            this.addSearchField = addSearchField;
            TypeConstraint = typeConstraint ?? new TypeConstraintStandard(null, TypeSettings.Class, false, false);
            groupingSettings = new TypesGroupSettings(TypeGrouping, typeConstraint, addEmptyValue);
        }


        private Type RetriveSelectedType(IReadOnlyList<Type> types, int selectedIndex)
        {
            if (addEmptyValue)
            {
                selectedIndex -= 1;
            }

            return selectedIndex >= 0 ? types[selectedIndex] : null;
        }


        //TODO: assign constraint here
        public void OnGui(Rect position, Type parentType, Type activeType, Action<Type> onSelect)
        {
            TypeConstraint.ApplyTarget(parentType);
            groupingSettings.Constraint = TypeConstraint;
            groupingSettings.Grouping = TypeGrouping;

            var info = TypeUtilities.GetGroupedInfo(groupingSettings);
            var types = info.Types;
            var options = info.Options;
            var index = info.IndexOf(activeType);

            var buttonLabel = new GUIContent(options[index]);
            if (addSearchField)
            {
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
                {
                    var type = RetriveSelectedType(types, i);
                    onSelect?.Invoke(type);
                });
            }
            else
            {
                using (new ZeroIndentScope())
                {
                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(position, index, options);
                    if (EditorGUI.EndChangeCheck())
                    {
                        var type = RetriveSelectedType(types, index);
                        onSelect?.Invoke(type);
                    }
                }
            }
        }


        public TypeConstraint TypeConstraint { get; set; }
        public TypeGrouping TypeGrouping { get; set; } = TypeGrouping.None;
    }
}