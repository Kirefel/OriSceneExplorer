using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(bool))]
    public class BoolEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            bool newValue = GUILayout.Toggle((bool)value, value.ToString());

            if (newValue != (bool)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
