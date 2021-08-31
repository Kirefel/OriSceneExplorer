using OriSceneExplorer.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public ComponentsView(int col, int row, int width, int height) : base(col, row, width, height, "GameObject")
        {
        }

        protected override void Draw(int windowID)
        {
            if (referenceGameObject != null)
            {
                GUILayout.TextField(fullPath);

                componentsScroll = GUILayout.BeginScrollView(componentsScroll);

                for (int i = 0; i < componentViews.Count; i++)
                {
                    var view = componentViews[i];

                    GUILayout.Label(view.ComponentName);
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
                            GUILayout.Label(kvp.Value.RawValue);
                        }

                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(30);
                }

                GUILayout.EndScrollView();
            }
        }

        private bool DrawButton(PropertyValue value)
        {
            if (GUILayout.Button(value.RawValue, "Label"))
            {
                var goref = new ViewerGORef()
                {
                    Name = value.Reference.gameObject.name,
                    Reference = value.Reference
                };
                Load(goref);
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

            var view = new ComponentView { ComponentName = component.GetType().Name };

            foreach (PropertyInfo propertyInfo in component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                ReflectionInfoWrapper wrapper = new ReflectionInfoWrapper(propertyInfo);
                if (wrapper.CanRead && !exclusions.Contains(propertyInfo.Name))
                    view.Values[propertyInfo.Name] = LoadMember(wrapper, component);
            }
            foreach (FieldInfo fieldInfo in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!exclusions.Contains(fieldInfo.Name))
                    view.Values[fieldInfo.Name] = LoadMember(new ReflectionInfoWrapper(fieldInfo), component);
            }

            return view;
        }

        private ComponentView LoadTransform(Transform transform)
        {
            var view = new ComponentView { ComponentName = "Transform" };
            view.Values["Position"] = new PropertyValue(transform.position.ToString(), typeof(Vector3));
            view.Values["Rotation"] = new PropertyValue(transform.eulerAngles.ToString(), typeof(Vector3));
            view.Values["Tag"] = new PropertyValue(transform.tag, typeof(string));

            return view;
        }

        private static string[] exclusions = new string[] { "transform", "gameObject", "name", "tag", "hideFlags", "useGUILayout", "isActiveAndEnabled" };
        private PropertyValue LoadMember(ReflectionInfoWrapper field, Component instance)
        {
            // Null -> null
            var objValue = field.GetValue(instance);
            if (objValue == null)
                return new PropertyValue("null", field.MemberType);

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

            // Raw value (string, int, bool etc.) -> draw value
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
            public bool CanRead => _fieldInfo != null ? true : _propertyInfo.CanRead;
        }

        public class ComponentView
        {
            public string ComponentName { get; set; }
            public Dictionary<string, PropertyValue> Values { get; set; } = new Dictionary<string, PropertyValue>();
        }

        public class PropertyValue
        {
            public string RawValue { get; set; }
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

            public PropertyValue(string rawValue, Type type)
            {
                RawValue = rawValue;
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