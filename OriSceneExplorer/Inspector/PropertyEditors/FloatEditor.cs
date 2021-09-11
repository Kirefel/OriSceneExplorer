using OriSceneExplorer.Inspector.PropertyEditors.Fields;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(float))]
    public class FloatEditor : PropertyEditor
    {
        FloatField field;

        public override bool Draw(ref object value)
        {
            if (field == null)
                field = new FloatField((float)value);

            float newValue = field.Draw((float)value, GUILayout.MaxWidth(ComponentsView.MaxValueWidth));

            if (newValue != (float)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
