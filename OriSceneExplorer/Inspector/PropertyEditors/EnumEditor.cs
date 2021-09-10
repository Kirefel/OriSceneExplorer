using System;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class EnumEditor : PropertyEditor
    {
        private readonly Type enumType;

        private bool shouldSet = false;
        private object setValue = null;

        public EnumEditor(Type enumType)
        {
            this.enumType = enumType;
        }

        public override bool Draw(ref object value)
        {
            if (GUILayout.Button(value.ToString() + " (enum)", "Label"))
            {
                var enumNames = Enum.GetNames(enumType);
                ContextMenu.Show(ChooseEnumCallback, enumNames);
            }

            if (shouldSet)
            {
                value = setValue;
                setValue = null;
                shouldSet = false;
                return true;
            }

            return false;
        }

        public void ChooseEnumCallback(int index)
        {
            if (index >= 0)
            {
                var enumValues = Enum.GetValues(enumType);
                var chosenValue = enumValues.GetValue(index);

                shouldSet = true;
                setValue = chosenValue;
            }
        }
    }
}
