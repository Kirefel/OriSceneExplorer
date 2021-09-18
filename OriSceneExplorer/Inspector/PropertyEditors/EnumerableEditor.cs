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

            GUILayout.Label($"{enumerable.Cast<object>().Count()} items");

            return false;
        }

        public override void DrawExpanded(object value)
        {
            var enumerable = value as IEnumerable;

            if (enumerable == null)
                return;

            int index = 0;
            foreach (var val in enumerable)
                GUILayout.Label($"{index++}: {val}");
        }
    }
}
