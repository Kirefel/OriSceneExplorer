using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class IntField
    {
        private string stringValue;
        private bool valid = true;
        public IntField(int value)
        {
            stringValue = value.ToString();
        }

        public int Draw(int value, params GUILayoutOption[] options)
        {
            Color c = GUI.backgroundColor;
            if (!valid)
                GUI.backgroundColor = Color.red;

            int retVal = value;
            stringValue = GUILayout.TextField(stringValue, options);

            if (GUI.changed)
            {
                if (int.TryParse(stringValue, out int i))
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
