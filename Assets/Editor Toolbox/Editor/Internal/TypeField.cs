using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    //TODO: refactor
    public class TypeField
    {
        private TypeConstraint constraint;
        private TypeAppearance appearance;


        public TypeField() : this(null, null)
        { }

        public TypeField(TypeConstraint constraint) : this(constraint, null)
        { }

        public TypeField(TypeConstraint constraint, TypeAppearance appearance)
        {
            this.constraint = constraint ?? new TypeConstraintStandard(null, TypeSettings.Class, false, false);
            this.appearance = appearance ?? new TypeAppearance(this.constraint, TypeGrouping.None , true);
        }


        private Type RetriveSelectedType(IReadOnlyList<Type> types, int selectedIndex, bool includeEmptyValue)
        {
            if (includeEmptyValue)
            {
                selectedIndex -= 1;
            }

            return selectedIndex >= 0 ? types[selectedIndex] : null;
        }


        public void OnGui(Rect position, bool addSearchField, Type activeType)
        {
            var info = TypeUtilities.GetGroupedInfo(Appearance);
            var values = info.Types;
            var labels = info.Labels;
            var index = info.IndexOf(activeType);

            var addEmptyValue = Appearance.AddEmptyValue;
            if (addSearchField)
            {
                var buttonLabel = new GUIContent(labels[index]);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, labels, (i) =>
                {
                    var type = RetriveSelectedType(values, i, addEmptyValue);
                    OnSelect?.Invoke(type);
                });
            }
            else
            {
                using (new ZeroIndentScope())
                {
                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(position, index, labels);
                    if (EditorGUI.EndChangeCheck())
                    {
                        var type = RetriveSelectedType(values, index, addEmptyValue);
                        OnSelect?.Invoke(type);
                    }
                }
            }
        }

        public void OnGui(Rect position, bool addSearchField, Type activeType, Type parentType)
        {
            Constraint.ApplyTarget(parentType);
            OnGui(position, addSearchField, activeType);
        }


        public TypeConstraint Constraint
        {
            get => constraint;
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException($"Cannot assign null constraint to the {nameof(TypeField)}.");
                }

                Appearance.Constraint = constraint = value;
            }
        }

        public TypeAppearance Appearance
        {
            get => appearance;
        }

        public Action<Type> OnSelect { get; set; }
    }
}