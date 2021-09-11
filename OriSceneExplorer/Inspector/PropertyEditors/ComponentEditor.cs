using System;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class ComponentEditor : PropertyEditor
    {
        private readonly Type componentType;

        public ComponentEditor(Type componentType)
        {
            this.componentType = componentType;
        }

        public override bool Draw(ref object value)
        {
            Component component = value as Component;

            if (GUILayout.Button(FormatString(value), "Label"))
            {
                if (component && Event.current.button == 0)
                {
                    // Left click = select
                    Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(component.gameObject));
                }
                else if (component && Event.current.button == 1 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                {
                    value = null;
                    return true;
                }
                else if (Event.current.button == 1 && ComponentSelection.Selection != null && componentType.IsAssignableFrom(ComponentSelection.Selection.GetType()))
                {
                    value = ComponentSelection.Selection;
                    return true;
                }
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
