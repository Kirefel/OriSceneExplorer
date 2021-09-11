using OriSceneExplorer.Inspector.PropertyEditors.Fields;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    [PropertyEditor(typeof(Rect))]
    public class RectEditor : PropertyEditor
    {
        Vector4Field field;

        public override bool Draw(ref object value)
        {
            if (field == null)
            {
                var rect = (Rect)value;
                field = new Vector4Field(new Vector4(rect.x, rect.y, rect.width, rect.height));
            }

            Vector4 newValue = field.Draw(GUILayout.MaxWidth(ComponentsView.MaxValueWidth));
            var newRect = new Rect(newValue.w, newValue.x, newValue.y, newValue.z);
            if (newRect != (Rect)value)
            {
                value = newRect;
                return true;
            }

            return false;
        }
    }
}
