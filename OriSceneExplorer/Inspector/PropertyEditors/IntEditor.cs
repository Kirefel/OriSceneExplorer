using OriSceneExplorer.Inspector.PropertyEditors.Fields;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(int))]
    public class IntEditor : PropertyEditor
    {
        IntField field;

        public override bool Draw(ref object value)
        {
            if (field == null)
                field = new IntField((int)value);

            int newValue = field.Draw((int)value, GUILayout.MaxWidth(ComponentsView.MaxValueWidth));

            if (newValue != (int)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
