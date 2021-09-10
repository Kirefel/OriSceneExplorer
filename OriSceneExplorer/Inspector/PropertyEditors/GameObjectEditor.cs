using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class GameObjectEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            GameObject gameObject = value as GameObject;

            if (gameObject)
            {
                if (GUILayout.Button(FormatString(value), "Label"))
                {
                    Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(gameObject));
                }
            }
            else
            {
                GUILayout.Label(FormatString(value));
            }

            return false;
        }

        public override string FormatString(object value)
        {
            var gameObject = value as GameObject;

            if (!gameObject)
                return "(null)";

            return $"{gameObject.name} (GameObject)";
        }
    }
}
