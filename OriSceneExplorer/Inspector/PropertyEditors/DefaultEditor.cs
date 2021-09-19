using System;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class DefaultEditor : PropertyEditor
    {
        private string cachedString;
        TypeInspector typeInspector;

        public DefaultEditor(Type type) : base(true, true)
        {
            if (type.Namespace == "System.Reflection")
                CanExpand = false; // yeah just... nah not touching that
            else if (type == typeof(AnimationCurve))
                CanExpand = false;
        }

        public override bool Draw(ref object value)
        {
            if (value == null)
                CanExpand = false; // This also disables actually reading the values immediately

            // These can't be changed so don't calculate label every frame
            if (cachedString == null)
            {
                cachedString = FormatString(value);
                if (CanExpand)
                    typeInspector = new TypeInspector(TypeDescriptorCache.GetDescriptor(value.GetType()), value);
            }

            GUILayout.Label(cachedString);

            return false;
        }

        public override void DrawExpanded(object value)
        {
            if (value != null && typeInspector != null)
            {
                typeInspector.Draw(false, value);
            }
        }
    }
}
