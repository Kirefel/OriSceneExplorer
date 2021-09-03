using OriSceneExplorer.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer
{
    public class ComponentsView : EditorView
    {
        ViewerGORef referenceGameObject = null;
        Vector2 componentsScroll = Vector2.zero;
        List<ComponentView> componentViews = new List<ComponentView>();
        string fullPath = null;

        private PropertyEditorFactory PropertyEditorFactory { get; }

        public event Action<ViewerGORef> OnFocusProperty;
        public event Action<GameObject> OnClone;
        public event Action<ViewerGORef> OnStartMoving;
        public event Action<ViewerGORef> OnStartRotating;

        string newComponentType = "";

        public ComponentsView(int col, int row, int width, int height) : base(col, row, width, height, "GameObject")
        {
            PropertyEditorFactory = new PropertyEditorFactory();
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

                for (int i = 0; i < componentViews.Count; i++)
                {
                    var view = componentViews[i];

                    DrawComponentHeader(view);
                    if (view.Expanded)
                    {
                        foreach (var kvp in view.Values)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(kvp.Key, GUILayout.Width(240));
                            GUILayout.Label(kvp.Value.TypeName, GUILayout.Width(160));

                            if (kvp.Value.Reference != null)
                            {
                                // Modifies collection so cancel now and get rebuilt next call
                                if (DrawButton(kvp.Value))
                                    break;
                            }
                            else
                            {
                                kvp.Value.Editor.Draw();
                                //GUILayout.Label(kvp.Value.StringValue);
                            }

                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.Space(30);
                }

                GUILayout.EndScrollView();
            }
        }

        private void DrawComponentHeader(ComponentView view)
        {
            GUILayout.BeginHorizontal();

            // Toggle button
            if (view.Component is MonoBehaviour mb)
            {
                mb.enabled = GUILayout.Toggle(mb.enabled, "", GUILayout.Width(16));
            }

            // Name
            if (GUILayout.Button(view.ComponentName, "Label"))
                view.Expanded = !view.Expanded;

            // Delete button
            if (view.Component.GetType() != typeof(Transform))
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    GameObject.Destroy(view.Component);
                    this.componentViews.Remove(view);
                }
            }

            GUILayout.EndHorizontal();
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
                foreach (var cv in this.componentViews)
                {
                    writer.WriteLine(cv.ComponentName);
                    int longestName = 0, longestType = 0;
                    foreach (var propView in cv.Values)
                    {
                        if (propView.Key.Length > longestName)
                            longestName = propView.Key.Length;
                        if (propView.Value.TypeName.Length > longestType)
                            longestType = propView.Value.TypeName.Length;
                    }

                    foreach (var propView in cv.Values)
                    {
                        writer.Write(propView.Key);
                        writer.Write(new string(' ', longestName - propView.Key.Length));
                        writer.Write("  ");

                        writer.Write(propView.Value.TypeName);
                        writer.Write(new string(' ', longestType - propView.Value.TypeName.Length));
                        writer.Write("  ");

                        int inset = longestName + longestType + 4;
                        writer.WriteLine(propView.Value.Editor.StringValue().Replace("\n", "\n" + new string(' ', inset)));
                    }

                    writer.WriteLine();
                }
                writer.WriteLine();
            }

            Debug.Log("Written to scene.txt");
        }

        private bool DrawButton(PropertyValue value)
        {
            if (GUILayout.Button(value.Editor.StringValue(), "Label"))
            {
                var goref = new ViewerGORef()
                {
                    Name = value.Reference.gameObject.name,
                    Reference = value.Reference
                };
                Load(goref);
                OnFocusProperty?.Invoke(goref);
                return true;
            }
            return false;
        }

        public void Load(ViewerGORef goref)
        {
            var go = goref.Reference;

            if (go)
            {
                referenceGameObject = goref;
                this.Title = goref.Name;
                this.fullPath = goref.GetFullPath();

                componentViews.Clear();
                var components = go.GetComponents<Component>();

                foreach (var component in components)
                {
                    componentViews.Add(LoadComponent(component));
                }
            }
        }

        private ComponentView LoadComponent(Component component)
        {
            if (typeof(Transform).IsAssignableFrom(component.GetType()))
                return LoadTransform(component as Transform);

            var view = new ComponentView { Component = component, ComponentName = component.GetType().Name };

            foreach (PropertyInfo propertyInfo in component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                ReflectionInfoWrapper wrapper = new ReflectionInfoWrapper(propertyInfo);
                if (wrapper.CanRead && !exclusions.Contains(propertyInfo.Name))
                    view.Values[propertyInfo.Name] = LoadEditor(wrapper, component);
            }
            foreach (FieldInfo fieldInfo in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!exclusions.Contains(fieldInfo.Name))
                    view.Values[fieldInfo.Name] = LoadEditor(new ReflectionInfoWrapper(fieldInfo), component);
            }

            return view;
        }

        private ComponentView LoadTransform(Transform transform)
        {
            var view = new ComponentView { Component = transform, ComponentName = "Transform" };
            view.Values["Position"] = new PropertyValue(new Vector3Editor(new ReflectionInfoWrapper(typeof(Transform).GetProperty("position", BindingFlags.Public | BindingFlags.Instance)), transform), typeof(Vector3));
            view.Values["Local Position"] = new PropertyValue(new Vector3Editor(new ReflectionInfoWrapper(typeof(Transform).GetProperty("localPosition", BindingFlags.Public | BindingFlags.Instance)), transform), typeof(Vector3));
            view.Values["Rotation"] = new PropertyValue(new Vector3Editor(new ReflectionInfoWrapper(typeof(Transform).GetProperty("eulerAngles", BindingFlags.Public | BindingFlags.Instance)), transform), typeof(Vector3));
            view.Values["Tag"] = new PropertyValue(transform.tag, typeof(string));

            return view;
        }

        private static string[] exclusions = new string[] { "transform", "gameObject", "name", "tag", "hideFlags", "useGUILayout", "isActiveAndEnabled", "enabled" };
        private PropertyValue LoadEditor(ReflectionInfoWrapper field, Component instance)
        {
            object objValue;
            try
            {
                objValue = field.GetValue(instance);
            }
            catch
            {
                return new PropertyValue(new ConstantEditor("(Unable to load value)"), field.MemberType);
            }

            // Null -> (null)
            if (objValue == null)
                return new PropertyValue("(null)", field.MemberType);

            // Component -> GameObjectName (ComponentType)
            if (typeof(Component).IsAssignableFrom(field.MemberType))
            {
                var componentValue = objValue as Component;
                return new PropertyValue($"{componentValue.gameObject.name} ({componentValue.GetType().Name})", field.MemberType) { Reference = componentValue.gameObject };
            }

            // GameObject -> GameObjectName
            if (typeof(GameObject).IsAssignableFrom(field.MemberType))
            {
                var goValue = objValue as GameObject;
                return new PropertyValue(goValue.name, field.MemberType) { Reference = goValue };
            }

            // List/Array -> [ 0, 1, 2 ]
            // Dictionary -> { Key.ToString : Value.ToString }
            if (typeof(IEnumerable).IsAssignableFrom(field.MemberType) && !typeof(string).IsAssignableFrom(field.MemberType))
            {
                var enumerableValue = objValue as IEnumerable;
                return new PropertyValue($"{enumerableValue.Cast<object>().Count()} items", field.MemberType);
            }

            // string, int, bool etc. -> draw value
            if (field.CanWrite)
                return new PropertyValue(PropertyEditorFactory.CreateEditor(field, instance), field.MemberType);
            else
                return new PropertyValue(objValue.ToString(), field.MemberType);
        }
    }

    namespace Reflection
    {
        public class ReflectionInfoWrapper
        {
            private readonly FieldInfo _fieldInfo;
            private readonly PropertyInfo _propertyInfo;

            public ReflectionInfoWrapper(FieldInfo fieldInfo) { _fieldInfo = fieldInfo; }
            public ReflectionInfoWrapper(PropertyInfo propertyInfo) { _propertyInfo = propertyInfo; }

            public Type MemberType => _fieldInfo?.FieldType ?? _propertyInfo.PropertyType;
            public object GetValue(object instance) => _fieldInfo != null ? _fieldInfo.GetValue(instance) : _propertyInfo.GetValue(instance, null);
            public void SetValue(object instance, object value)
            {
                if (_fieldInfo != null)
                    _fieldInfo.SetValue(instance, value);
                else
                    _propertyInfo.SetValue(instance, value, null);
            }

            public bool CanRead => _fieldInfo != null || _propertyInfo.CanRead;
            public bool CanWrite => _fieldInfo != null || _propertyInfo.CanWrite;
        }

        public class ComponentView
        {
            public Component Component { get; set; }
            public string ComponentName { get; set; }
            public Dictionary<string, PropertyValue> Values { get; set; } = new Dictionary<string, PropertyValue>();
            public bool Expanded { get; set; } = true;
        }

        public class PropertyValue
        {
            public IPropertyEditor Editor { get; }
            public string TypeName { get; set; }

            private WeakReference reference;
            public GameObject Reference
            {
                get
                {
                    if (reference == null || !reference.IsAlive)
                        return null;
                    return reference.Target as GameObject;
                }
                set
                {
                    reference = new WeakReference(value);
                }
            }

            public PropertyValue(string constantValue, Type type)
            {
                Editor = new ConstantEditor(constantValue);
                TypeName = GetFormattedName(type);
            }

            public PropertyValue(IPropertyEditor editor, Type type)
            {
                Editor = editor;
                TypeName = GetFormattedName(type);
            }

            private static string GetFormattedName(Type type)
            {
                if (type.IsGenericType)
                {
                    string genericArguments = type.GetGenericArguments()
                                        .Select(x => x.Name)
                                        .Aggregate((x1, x2) => $"{x1}, {x2}");
                    return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}"
                         + $"<{genericArguments}>";
                }
                return type.Name;
            }
        }
    }
}