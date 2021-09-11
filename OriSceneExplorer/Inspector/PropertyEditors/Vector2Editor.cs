using OriSceneExplorer.Inspector.PropertyEditors.Fields;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(Vector2))]
    public class Vector2Editor : PropertyEditor
    {
        Vector2Field field;

        public override bool Draw(ref object value)
        {
            if (field == null)
                field = new Vector2Field((Vector2)value);


            Vector2 newValue = field.Draw(GUILayout.MaxWidth(ComponentsView.MaxValueWidth));
            if (newValue != (Vector2)value)
            {
                value = newValue;
                return true;
            }

            return false;
        }
    }
}
