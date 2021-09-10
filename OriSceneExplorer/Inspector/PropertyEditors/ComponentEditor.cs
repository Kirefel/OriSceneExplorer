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
                    Dispatch.Queue(() => Editor.Instance.NavigateToGameObject(component.gameObject));
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
