using System.Collections;
using System.Linq;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class EnumerableEditor : PropertyEditor
    {
        public EnumerableEditor() : base(true, true) { }

        public override bool Draw(ref object value)
        {
            if (value == null)
            {
                GUILayout.Label("(null)");
                return false;
            }

            var enumerable = value as IEnumerable;
            if (enumerable == null)
            {
                Debug.Log("enumerable is null");
                return false;
            }

            if (this.Expanded)
            {
                int index = 0;
                foreach (var val in enumerable)
                    GUILayout.Label($"{index++}: {val}");
            }
            else
            {
                GUILayout.Label($"{enumerable.Cast<object>().Count()} items");
            }

            return false;
        }
    }
}
