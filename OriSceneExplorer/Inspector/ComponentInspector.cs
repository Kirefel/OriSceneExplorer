using System.IO;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class ComponentInspector
    {
        public string Name { get; }
        private Component componentInstance;

        private bool expanded = true;
        private readonly bool canDelete = false;
        private TypeInspector typeInspector;

        public ComponentInspector(string name, Component componentInstance, TypeDescriptor componentDescriptor)
        {
            Name = name;
            this.componentInstance = componentInstance;
            canDelete = !(componentInstance is Transform);
            typeInspector = new TypeInspector(componentDescriptor, componentInstance);
        }

        public void Draw()
        {
            DrawHeader();

            if (!expanded)
                return;

            bool editable = true;
            if (!componentInstance)
            {
                GUI.color = Color.red;
                GUILayout.Label("Component has been destroyed");
                GUI.color = Color.white;
                editable = false;
            }

            typeInspector.Draw(editable, componentInstance);

            GUILayoutEx.HorizontalDivider();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();

            // Toggle button
            if (componentInstance && componentInstance is MonoBehaviour mb)
                mb.enabled = GUILayout.Toggle(mb.enabled, "", GUILayout.Width(16));

            // Name
            if (GUILayout.Button(Name, "Label"))
            {
                if (Event.current.button == 0)
                    expanded = !expanded;
                else if (Event.current.button == 1)
                    ComponentSelection.Selection = componentInstance;
            }

            // Delete button
            if (componentInstance && canDelete)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Object.Destroy(componentInstance);
                    componentInstance = null;
                }
            }

            GUILayout.EndHorizontal();
        }

        public void WriteToStream(StreamWriter stream)
        {
            //stream.WriteLine(Name);
            //int longestName = 0, longestType = 0;
            //foreach (var propInspector in propertyInspectors)
            //{
            //    if (propInspector.Descriptor.Name.Length > longestName)
            //        longestName = propInspector.Descriptor.Name.Length;
            //    if (propInspector.Descriptor.TypeName.Length > longestType)
            //        longestType = propInspector.Descriptor.TypeName.Length;
            //}

            //foreach (var propInspector in propertyInspectors)
            //{
            //    stream.Write(propInspector.Descriptor.Name);
            //    stream.Write(new string(' ', longestName - propInspector.Descriptor.Name.Length));
            //    stream.Write("  ");

            //    stream.Write(propInspector.Descriptor.TypeName);
            //    stream.Write(new string(' ', longestType - propInspector.Descriptor.TypeName.Length));
            //    stream.Write("  ");

            //    int inset = longestName + longestType + 4;
            //    string value = propInspector.GetStringValue();
            //    stream.WriteLine(value.Replace("\n", "\n" + new string(' ', inset)));
            //}

            //stream.WriteLine();
        }
    }
}