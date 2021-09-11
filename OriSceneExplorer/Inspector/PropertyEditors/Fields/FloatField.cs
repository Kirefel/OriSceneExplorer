using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class FloatField
    {
        private string stringValue;
        private bool valid = true;
        public FloatField(float value)
        {
            stringValue = value.ToString();
        }

        public float Draw(float value, params GUILayoutOption[] options)
        {
            Color c = GUI.backgroundColor;
            if (!valid)
                GUI.backgroundColor = Color.red;

            float retVal = value;
            stringValue = GUILayout.TextField(stringValue, options);

            if (GUI.changed)
            {
                if (float.TryParse(stringValue, out float i))
                {
                    retVal = i;
                    valid = true;
                }
                else
                {
                    valid = false;
                }
            }

            GUI.backgroundColor = c;
            return retVal;
        }
    }
}
