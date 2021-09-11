using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class Vector2Field
    {
        private string stringValue;
        private bool valid = true;
        private Vector2 currentVector = Vector2.zero;
        private Vector2 lastValidVector = Vector2.zero;

        public Vector2Field(Vector2 value)
        {
            stringValue = $"{value.x}, {value.y}";
            currentVector = value;
            lastValidVector = value;
        }

        public Vector2 Draw(params GUILayoutOption[] options)
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

        private bool TryParse(string value, ref Vector2 vector)
        {
            var components = value.Split(',');

            if (components.Length != 2)
                return false;

            if (float.TryParse(components[0], out vector.x)
                && float.TryParse(components[1], out vector.y))
            {
                return true;
            }

            return false;
        }
    }
}
