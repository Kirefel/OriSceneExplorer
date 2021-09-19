using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class Vector4Field
    {
        private string stringValue;
        private bool valid = true;
        private Vector4 currentVector = Vector4.zero;
        private Vector4 lastValidVector = Vector4.zero;

        public Vector4Field(Vector4 value)
        {
            stringValue = $"{value.x}, {value.y}, {value.z}, {value.w}";
            currentVector = value;
            lastValidVector = value;
        }

        public Vector4 Draw(params GUILayoutOption[] options)
        {
            Color c = GUI.backgroundColor;
            if (!valid)
                GUI.backgroundColor = Color.red;

            stringValue = GUILayout.TextField(stringValue, options);

            if (GUI.changed)
            {
                if (TryParse(stringValue, ref currentVector))
                {
                    valid = true;
                    lastValidVector = currentVector;
                }
                else
                {
                    valid = false;
                }
            }

            GUI.backgroundColor = c;
            return lastValidVector;
        }

        private bool TryParse(string value, ref Vector4 vector)
        {
            var components = value.Split(',');

            if (components.Length != 4)
                return false;

            if (float.TryParse(components[0], out vector.x)
                && float.TryParse(components[1], out vector.y)
                && float.TryParse(components[2], out vector.z)
                && float.TryParse(components[3], out vector.w))
            {
                return true;
            }

            return false;
        }
    }
}
