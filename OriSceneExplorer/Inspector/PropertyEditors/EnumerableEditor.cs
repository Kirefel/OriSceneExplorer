using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public class EnumerableEditor : PropertyEditor
    {
        PropertyEditor[] editors = null;
        private readonly bool useEditors = false;
        private readonly Type enumerableType;

        public EnumerableEditor(Type type) : base(true, true)
        {
            if (typeof(IDictionary).IsAssignableFrom(type)) // should be handled elsewhere as it's enumerable on KVP
                return;

            if (type.IsGenericType) // e.g. List<int>
            {
                enumerableType = type.GetGenericArguments()[0];
                useEditors = true;
            }
            else if (type.GetElementType() != null) // e.g. int[]
            {
                enumerableType = type.GetElementType();
                useEditors = true;
            }
        }

        public override bool Draw(ref object value)
        {
            if (value == null)
            {
                GUILayout.Label("(null)");
                return false;
            }

            var enumerable = value as IEnumerable;

            if (useEditors && editors == null)
            {
                var arr = enumerable.Cast<object>();
                int count = arr.Count();
                editors = new PropertyEditor[count];
                for (int i = 0; i < count; i++)
                {
                    var element = arr.ElementAt(i);
                    if (element != null)
                        editors[i] = PropertyEditorFactory.CreateEditor(enumerableType);
                }
            }

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
            {
                if (val == null || (useEditors && editors[index] == null))
                {
                    GUILayout.Label($"{index}: (null)");
                }
                else if (useEditors)
                {
                    var editor = editors[index];
                    if (editor.Expanded)
                    {
                        if (GUILayout.Button($"- {index}:", "Label", GUILayout.Width(32)))
                            editor.Expanded = false;

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(32);
                        GUILayout.BeginVertical();
                        editor.DrawExpanded(val);
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        if (editor.CanExpand)
                        {
                            if (GUILayout.Button($"+ {index}:", "Label", GUILayout.Width(32)))
                                editor.Expanded = true;
                        }
                        else
                        {
                            GUILayout.Label($"{index}:", GUILayout.Width(32));
                        }

                        object valref = val;
                        editor.Draw(ref valref); // promise not to edit it
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label($"{index}: {val}");
                }
                index++;
            }
        }
    }
}
