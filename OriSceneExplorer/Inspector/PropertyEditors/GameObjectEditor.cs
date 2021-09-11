using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class GameObjectEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            GameObject gameObject = value as GameObject;

            if (GUILayout.Button(FormatString(value), "Label"))
            {
                if (gameObject && Event.current.button == 0)
                {
                    // Left click = select
                    Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(gameObject));
                }
                else if (gameObject && Event.current.button == 1 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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
