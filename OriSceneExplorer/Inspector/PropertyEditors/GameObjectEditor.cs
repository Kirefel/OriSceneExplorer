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
                    if (Event.current.button == 0)
                    {
                        // Left click = select
                        Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(gameObject));
                    }
                    else if (Event.current.button == 1 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                    {
                        value = null;
                        return true;
                    }
                    else
                    {
                        var selectionGameObject = ComponentSelection.Selection?.gameObject;
                        if (selectionGameObject)
                        {
                            value = selectionGameObject;
                            return true;
                        }
                    }
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
