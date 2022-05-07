using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Internal
{
    /// <summary>
    /// Out-of-the-box field that can be used as a <see cref="Type"/> picker.
    /// </summary>
    public class TypeField
    {
        private TypeConstraintContext constraintContext;
        private TypeAppearanceContext appearanceContext;


        public TypeField() : this(null, null)
        { }

        public TypeField(TypeConstraintContext constraintContext) : this(constraintContext, null)
        { }

        public TypeField(TypeConstraintContext constraintContext, TypeAppearanceContext appearanceContext)
        {
            this.constraintContext = constraintContext ?? new TypeConstraintStandard(null, TypeSettings.Class, false, false);
            this.appearanceContext = appearanceContext ?? new TypeAppearanceContext(this.constraintContext, TypeGrouping.None, true);
        }


        private Type RetriveSelectedType(IReadOnlyList<Type> types, int selectedIndex, bool includeEmptyValue)
        {
            if (includeEmptyValue)
            {
                selectedIndex -= 1;
            }

            return selectedIndex >= 0 ? types[selectedIndex] : null;
        }


        public void OnGui(Rect position, bool addSearchField, Action<Type> onSelect)
        {
            OnGui(position, addSearchField, onSelect, null);
        }

        public void OnGui(Rect position, bool addSearchField, Action<Type> onSelect, Type activeType)
        {
            var collection = TypeUtilities.GetCollection(AppearanceContext);
            var values = collection.Values;
            var labels = collection.Labels;
            var index = collection.IndexOf(activeType);

            var addEmptyValue = AppearanceContext.AddEmptyValue;
            if (addSearchField)
            {
                var buttonLabel = new GUIContent(labels[index]);
                ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, labels, (i) =>
                {
                    var type = RetriveSelectedType(values, i, addEmptyValue);
                    onSelect?.Invoke(type);
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
                        onSelect?.Invoke(type);
                    }
                }
            }
        }

        public void OnGui(Rect position, bool addSearchField, Action<Type> onSelect, Type activeType, Type parentType)
        {
            ConstraintContext.ApplyTarget(parentType);
            OnGui(position, addSearchField, onSelect, activeType);
        }


        public TypeConstraintContext ConstraintContext
        {
            get => constraintContext;
            set
            {
                constraintContext = value ?? throw new NullReferenceException($"Cannot assign null constraint to the {nameof(TypeField)}.");
                AppearanceContext.Constraint = constraintContext;
            }
        }

        public TypeAppearanceContext AppearanceContext
        {
            get => appearanceContext;
            set
            {
                appearanceContext = value ?? throw new NullReferenceException($"Cannot assign null appearance to the {nameof(TypeField)}.");
                ConstraintContext = appearanceContext.Constraint;
            }
        }
    }
}