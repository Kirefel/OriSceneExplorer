﻿using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class Vector3Field
    {
        private string stringValue;
        private bool valid = true;
        private Vector3 currentVector = Vector3.zero;
        private Vector3 lastValidVector = Vector3.zero;

        public Vector3Field(Vector3 value)
        {
            stringValue = $"{value.x}, {value.y}, {value.z}";
            currentVector = value;
            lastValidVector = value;
        }

        public Vector3 Draw(params GUILayoutOption[] options)
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

        private bool TryParse(string value, ref Vector3 vector)
        {
            var components = value.Split(',');

            if (components.Length != 3)
                return false;

            if (float.TryParse(components[0], out vector.x)
                && float.TryParse(components[1], out vector.y)
                && float.TryParse(components[2], out vector.z))
            {
                return true;
            }

            return false;
        }
    }
}
