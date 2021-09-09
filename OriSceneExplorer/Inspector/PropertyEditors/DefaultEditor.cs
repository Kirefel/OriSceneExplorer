using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class DefaultEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            GUILayout.Label(FormatString(value));
            return false;
        }
    }
}