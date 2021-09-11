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
        private bool error = false;

        public PropertyInspector(PropertyDescriptor descriptor, Component instance)
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

        public void Draw(bool editable, Component instance)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(Descriptor.Name, GUILayout.Width(OriSceneExplorer.Editor.EditorSettings.PropertyNameColumnWidth));
            GUILayout.Label(Descriptor.TypeName, GUILayout.Width(OriSceneExplorer.Editor.EditorSettings.PropertyTypeColumnWidth));

            if (error)
            {
                GUILayout.Label("(error occurred getting value)");
            }
            else if (!editable || !Descriptor.Info.CanWrite || !instance)
            {
                // Readonly mode
                GUILayout.Label(GetStringValue());
            }
            else
            {
                // Writable property (update cached value, then synchronise)
                if (Editor.Draw(ref cachedValue))
                    Descriptor.Info.SetValue(instance, cachedValue);
            }

            GUILayout.EndHorizontal();
        }
        public string GetStringValue() => Editor.FormatString(cachedValue);
    }
}