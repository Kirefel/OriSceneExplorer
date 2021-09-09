using OriSceneExplorer.Inspector.PropertyEditors.Fields;

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

            int newValue = field.Draw((int)value);

            if (newValue != (int)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
