using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class TypeDescriptor
    {
        private readonly string componentName;
        public List<PropertyDescriptor> Properties { get; }

        private static readonly string[] exclusions = new string[] { "transform", "gameObject", "name", "tag", "hideFlags", "useGUILayout", "isActiveAndEnabled", "enabled" };

        public List<MethodDescriptor> Methods { get; }

        public TypeDescriptor(Type componentType)
        {
            componentName = componentType.Name;
            Properties = new List<PropertyDescriptor>();
            Methods = new List<MethodDescriptor>();

            foreach (PropertyInfo propertyInfo in componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                ReflectionInfoWrapper wrapper = new ReflectionInfoWrapper(propertyInfo);
                if (wrapper.CanRead && !ShouldExclude(componentType, propertyInfo.Name) && !wrapper.IsIndex)
                    Properties.Add(new PropertyDescriptor(wrapper));
            }
            foreach (FieldInfo fieldInfo in componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!ShouldExclude(componentType, fieldInfo.Name) && !fieldInfo.Name.EndsWith("k__BackingField"))
                    Properties.Add(new PropertyDescriptor(new ReflectionInfoWrapper(fieldInfo)));
            }
            foreach (MethodInfo methodInfo in componentType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.GetParameters().Length == 0))
            {
                if (methodInfo.Name.StartsWith("__BTN__"))
                    Methods.Add(new MethodDescriptor { Name = methodInfo.Name.Substring(7), Method = methodInfo });
                else if (methodInfo.GetCustomAttribute<UnityEngine.ContextMenu>(false, out var att))
                    Methods.Add(new MethodDescriptor { Name = att.menuItem, Method = methodInfo });
            }
        }

        private static bool ShouldExclude(Type type, string name) => typeof(UnityEngine.Object).IsAssignableFrom(type) && exclusions.Contains(name);

        public TypeDescriptor(Type componentType, params string[] propertyNames)
        {
            componentName = componentType.Name;
            Properties = new List<PropertyDescriptor>();
            Methods = new List<MethodDescriptor>();

            foreach (string name in propertyNames)
            {
                var infoWrapper = new ReflectionInfoWrapper(componentType, name);
                if (!infoWrapper.CanRead)
                    throw new InvalidOperationException($"{name} is not a readable property or field of {componentName}");

                Properties.Add(new PropertyDescriptor(infoWrapper));
            }
        }

        public ComponentInspector BuildInspector(Component component)
        {
            return new ComponentInspector(componentName, component, this);
        }
    }
}