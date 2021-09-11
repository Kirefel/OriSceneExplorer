using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class ComponentEditor : PropertyEditor
    {
        public override bool Draw(ref object value)
        {
            Component component = value as Component;

            if (component)
            {
                if (GUILayout.Button(FormatString(value), "Label"))
                {
                    if (Event.current.button == 0)
                    {
                        // Left click = select
                        Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(component.gameObject));
                    }
                    else if (Event.current.button == 1 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                    {
                        value = null;
                        return true;
                    }
                    else if (Event.current.button == 1 && ComponentSelection.Selection != null && component.GetType().IsAssignableFrom(ComponentSelection.Selection.GetType()))
                    {
                        value = ComponentSelection.Selection;
                        return true;
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
            var component = value as Component;

            if (!component)
                return "(null)";

            return $"{component.gameObject.name} ({component.GetType().Name})";
        }
    }
}
