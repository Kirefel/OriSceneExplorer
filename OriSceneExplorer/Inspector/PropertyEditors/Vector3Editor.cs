using OriSceneExplorer.Inspector.PropertyEditors.Fields;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(Vector3))]
    public class Vector3Editor : PropertyEditor
    {
        Vector3Field field;

        public override bool Draw(ref object value)
        {
            if (field == null)
                field = new Vector3Field((Vector3)value);


            Vector3 newValue = field.Draw((Vector3)value);
            if (newValue != (Vector3)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
