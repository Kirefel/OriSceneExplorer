using System.Collections;
using System.Linq;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class DefaultEditor : PropertyEditor
    {
        private string cachedString;

        public override bool Draw(ref object value)
        {
            // These can't be changed so don't calculate label every frame
            if (cachedString == null)
                cachedString = FormatString(value);

            GUILayout.Label(cachedString);
            return false;
        }

        public override string FormatString(object value)
        {
            // Null -> (null)
            if (value == null)
                return "(null)";

            // List/Array -> [ 0, 1, 2 ]
            // Dictionary -> { Key.ToString : Value.ToString }
            if (typeof(IEnumerable).IsAssignableFrom(value.GetType()) && !typeof(string).IsAssignableFrom(value.GetType()))
            {
                var enumerableValue = value as IEnumerable;
                return $"{enumerableValue.Cast<object>().Count()} items";
            }

            return base.FormatString(value);
        }
    }
}
