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

        public ComponentsView(int col, int row, int width, int height) : base(col, row, width, height, "GameObject")
        {
        }

        protected override void Draw(int windowID)
        {
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
                newComponentType = GUILayout.TextField(newComponentType);
                if (GUILayout.Button("Add Component", GUILayout.Width(240)))
                {
                    var type = Assembly.GetAssembly(typeof(SeinCharacter)).GetType(newComponentType)
                            ?? Assembly.GetAssembly(typeof(GameObject)).GetType(newComponentType);
                    if (type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        referenceGameObject.Reference.AddComponent(type);
                        Load(referenceGameObject);
                    }
                }
                GUILayout.EndHorizontal();

                componentsScroll = GUILayout.BeginScrollView(componentsScroll);

                foreach (var inspector in inspectors)
                {
                    inspector.Draw();
                    GUILayout.Space(30);
                }
                //for (int i = 0; i < componentViews.Count; i++)
                //{
                //    var view = componentViews[i];

                //    DrawComponentHeader(view);
                //    if (view.Expanded)
                //    {
                //        foreach (var kvp in view.Values)
                //        {
                //            GUILayout.BeginHorizontal();
                //            GUILayout.Label(kvp.Key, GUILayout.Width(240));
                //            GUILayout.Label(kvp.Value.TypeName, GUILayout.Width(160));

                //            if (kvp.Value.Reference != null)
                //            {
                //                // Modifies collection so cancel now and get rebuilt next call
                //                if (DrawButton(kvp.Value))
                //                    break;
                //            }
                //            else
                //            {
                //                //if (kvp.Value.Reference != null)
                //                kvp.Value.Editor.Draw();
                //                //else
                //                //    GUILayout.Label(kvp.Value.Editor.StringValue());
                //            }

                //            GUILayout.EndHorizontal();
                //        }
                //    }

                //    GUILayout.Space(30);
                //}

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

        //private bool DrawButton(PropertyValue value)
        //{
        //    if (GUILayout.Button(value.Editor.StringValue(), "Label"))
        //    {
        //        var goref = new ViewerGORef()
        //        {
        //            Name = value.Reference.gameObject.name,
        //            Reference = value.Reference
        //        };
        //        Load(goref);
        //        OnFocusProperty?.Invoke(goref);
        //        return true;
        //    }
        //    return false;
        //}

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
                    var descriptor = ComponentDescriptorCache.GetDescriptor(component.GetType());
                    inspectors.Add(descriptor.BuildInspector(component));
                }
            }
        }

    //    private static string[] exclusions = new string[] { "transform", "gameObject", "name", "tag", "hideFlags", "useGUILayout", "isActiveAndEnabled", "enabled" };
    //    private PropertyValue LoadEditor(ReflectionInfoWrapper field, Component instance)
    //    {
    //        object objValue;
    //        try
    //        {
    //            objValue = field.GetValue(instance);
    //        }
    //        catch
    //        {
    //            return new PropertyValue(new ConstantEditor("(Unable to load value)"), field.Type);
    //        }

    //        // Null -> (null)
    //        if (objValue == null)
    //            return new PropertyValue("(null)", field.Type);

    //        // Component -> GameObjectName (ComponentType)
    //        if (typeof(Component).IsAssignableFrom(field.Type))
    //        {
    //            var componentValue = objValue as Component;
    //            return new PropertyValue($"{componentValue.gameObject.name} ({componentValue.GetType().Name})", field.Type) { Reference = componentValue.gameObject };
    //        }

    //        // GameObject -> GameObjectName
    //        if (typeof(GameObject).IsAssignableFrom(field.Type))
    //        {
    //            var goValue = objValue as GameObject;
    //            return new PropertyValue(goValue.name, field.Type) { Reference = goValue };
    //        }

    //        // List/Array -> [ 0, 1, 2 ]
    //        // Dictionary -> { Key.ToString : Value.ToString }
    //        if (typeof(IEnumerable).IsAssignableFrom(field.Type) && !typeof(string).IsAssignableFrom(field.Type))
    //        {
    //            var enumerableValue = objValue as IEnumerable;
    //            return new PropertyValue($"{enumerableValue.Cast<object>().Count()} items", field.Type);
    //        }

    //        // string, int, bool etc. -> draw value
    //        if (field.CanWrite)
    //            return new PropertyValue(PropertyEditorFactory.CreateEditor(field, instance), field.Type);
    //        else
    //            return new PropertyValue(objValue.ToString(), field.Type);
    //    }
    //}
    }
}