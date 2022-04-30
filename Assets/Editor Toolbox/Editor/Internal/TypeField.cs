using System;

using UnityEngine;

namespace Toolbox.Editor.Internal
{
    public class TypeField
    {
        private readonly bool addEmptyValue;
        private readonly bool addSearchField;
        private readonly TypeConstraint typeConstraint;


        public TypeField(bool addEmptyValue = true, bool addSearchField = true, bool allowAbstract = false, bool allowObsolete = false)
            : this(addEmptyValue, addSearchField, new TypeConstraintStandard(null, allowAbstract, allowObsolete))
        { }

        public TypeField(bool addEmptyValue = true, bool addSearchField = true, TypeConstraint typeConstraint = null)
        {
            this.addEmptyValue = addEmptyValue;
            this.addSearchField = addSearchField;
            this.typeConstraint = typeConstraint ?? new TypeConstraintStandard(null, false, false);
        }


        public void OnGui(Rect position, Type parentType, Type activeType, Action<Type> onSelect)
        {
            typeConstraint.ApplyTarget(parentType);
            var types = TypeUtilities.GetTypes(typeConstraint);

            //TODO: cache options
            //TODO: add empty option
            var itemsCount = types.Count;
            var options = new string[itemsCount];
            var index = 0;
            for (var i = 0; i < itemsCount; i++)
            {
                var menuType = types[i];
                var menuName = menuType.Name;
                if (menuType == activeType)
                {
                    index = i;
                }

                options[i] = menuName;
            }

            var buttonLabel = new GUIContent(options[index]);
            ToolboxEditorGui.DrawSearchablePopup(position, buttonLabel, index, options, (i) =>
            {
                onSelect?.Invoke(i >= 0 ? types[i] : null);
            });
        }
    }
}