using OriSceneExplorer.Inspector.PropertyEditors;
using System;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class PropertyInspector
    {
        public PropertyDescriptor Descriptor { get; }
        public PropertyEditor Editor { get; }
        private object cachedValue;
        private readonly bool error = false;

        public PropertyInspector(PropertyDescriptor descriptor, object instance)
        {
            Descriptor = descriptor;
            Editor = PropertyEditorFactory.CreateEditor(descriptor);
            try
            {
                cachedValue = descriptor.Info.GetValue(instance);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                error = true;
            }
        }

        public void Draw(bool editable, object instance)
        {
            GUILayout.BeginHorizontal();

            // Name is a click-to-toggle if an enumerable or inspectable type
            if (Editor.CanExpand)
            {
                string prefix = Editor.Expanded ? "- " : "+ ";
                if (GUILayout.Button(prefix + Descriptor.Name, "Label", GUILayout.Width(OriSceneExplorer.Editor.EditorSettings.PropertyNameColumnWidth)))
                    Editor.Expanded = !Editor.Expanded;
            }
            else
            {
                GUILayout.Label(Descriptor.Name, GUILayout.Width(OriSceneExplorer.Editor.EditorSettings.PropertyNameColumnWidth));
            }

            GUILayout.Label(Descriptor.TypeName, GUILayout.Width(OriSceneExplorer.Editor.EditorSettings.PropertyTypeColumnWidth));

            if (error)
            {
                GUILayout.Label("(error occurred getting value)");
                GUILayout.EndHorizontal();
            }
            else if (((!editable || !Descriptor.Info.CanWrite) && !Editor.DrawWhenReadonly) || instance == null || (instance is Component c && !c))
            {
                // Readonly mode
                GUILayout.Label(GetStringValue());
                GUILayout.EndHorizontal();
            }
            else
            {
                // Writable property (update cached value, then synchronise)
                if (Editor.Draw(ref cachedValue))
                    Descriptor.Info.SetValue(instance, cachedValue);
                GUILayout.EndHorizontal();

                if (Editor.Expanded)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(32);
                    GUILayout.BeginVertical();
                    Editor.DrawExpanded(cachedValue);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
        public string GetStringValue() => Editor.FormatString(cachedValue);
    }
}