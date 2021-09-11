using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(string))]
    public class StringEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            string str = (string)value;
            if (value == null)
                str = "";

            string newValue = GUILayout.TextField(str, GUILayout.MaxWidth(ComponentsView.MaxValueWidth));

            if (newValue != str)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
