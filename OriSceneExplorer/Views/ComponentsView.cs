using OriSceneExplorer.Inspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer
{
    public class ComponentsView : EditorView
    {
        ViewerGORef referenceGameObject = null;
        Vector2 componentsScroll = Vector2.zero;
        List<ComponentInspector> inspectors = new List<ComponentInspector>();
        string fullPath = null;

        public event Action<ViewerGORef> OnFocusProperty;
        public event Action<GameObject> OnClone;
        public event Action<ViewerGORef> OnStartMoving;
        public event Action<ViewerGORef> OnStartRotating;

        string newComponentType = "";
        public static float MaxValueWidth;

        public ComponentsView(int col, int row, int width, int height) : base(col, row, width, height, "GameObject")
        {
        }

        protected override void Draw(int windowID)
        {
            MaxValueWidth = this.windowRect.width - Editor.EditorSettings.PropertyNameColumnWidth - Editor.EditorSettings.PropertyTypeColumnWidth - 55; // 55 for scroll bar and insets

            if (referenceGameObject != null)
            {
                GUILayout.TextField(fullPath);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Dump to file"))
                    DumpToFile();
                if (GUILayout.Button("Clone"))
                    Clone();
                if (GUILayout.Button("Move"))
                    OnStartMoving?.Invoke(referenceGameObject);
                if (GUILayout.Button("Rotate"))
                    OnStartRotating?.Invoke(referenceGameObject);
                if (GUILayout.Button("Refresh") && referenceGameObject?.Reference)
                    Load(referenceGameObject);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                newComponentType = GUILayout.TextField(newComponentType, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Add Component", GUILayout.ExpandWidth(false)))
                {
                    var type = Assembly.GetAssembly(typeof(SeinCharacter)).GetType(newComponentType)
                            ?? Assembly.GetAssembly(typeof(GameObject)).GetType(newComponentType);
                    if (type != null && typeof(Component).IsAssignableFrom(type))
                    {
                        referenceGameObject.Reference.AddComponent(type);
                        Load(referenceGameObject);
                        Debug.Log($"Added {type.Name} to {referenceGameObject.Name}");
                    }
                    else
                    {
                        Debug.Log($"The type {newComponentType} could not be found. Ensure it derives from Component and the namespace is included.");
                    }
                }
                GUILayout.EndHorizontal();

                if (ComponentSelection.Selection != null)
                {
                    GUILayout.Label($"Ready to set value to {ComponentSelection.Selection.name} ({ComponentSelection.Selection.GetType().Name})");
                }

                GUILayout.Space(20);

                componentsScroll = GUILayout.BeginScrollView(componentsScroll);

                foreach (var inspector in inspectors)
                {
                    inspector.Draw();
                    GUILayout.Space(30);
                }

                GUILayout.EndScrollView();
            }
        }

        private void Clone()
        {
            if (referenceGameObject.Reference == null)
                return;

            var clone = GameObject.Instantiate(referenceGameObject.Reference);
            clone.transform.SetParent(referenceGameObject.Reference.transform.parent, true);
            clone.transform.position = referenceGameObject.Reference.transform.position;
            OnClone?.Invoke(clone.gameObject);
        }

        private void DumpToFile()
        {
            using (var writer = new StreamWriter("scene.txt", append: true))
            {
                writer.WriteLine("==============================");
                writer.WriteLine(referenceGameObject.Name);
                writer.WriteLine(fullPath);
                foreach (var inspector in inspectors)
                {
                    inspector.WriteToStream(writer);
                }
                writer.WriteLine();
            }

            Debug.Log("Written to scene.txt");
        }

        public void NavigateToGameObject(GameObject go)
        {
            var goref = new ViewerGORef()
            {
                Name = go.name,
                Reference = go
            };
            Load(goref);
            OnFocusProperty?.Invoke(goref);
        }

        public void Load(ViewerGORef goref)
        {
            var go = goref.Reference;

            if (go)
            {
                referenceGameObject = goref;
                this.Title = goref.Name;
                this.fullPath = goref.GetFullPath();

                inspectors.Clear();
                var components = go.GetComponents<Component>();

                foreach (var component in components)
                {
                    var descriptor = TypeDescriptorCache.GetDescriptor(component.GetType());
                    inspectors.Add(descriptor.BuildInspector(component));
                }
            }
        }
    }
}