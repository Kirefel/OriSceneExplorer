using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class ComponentDescriptor
    {
        private readonly string componentName;
        public List<PropertyDescriptor> Properties { get; }

        private static readonly string[] exclusions = new string[] { "transform", "gameObject", "name", "tag", "hideFlags", "useGUILayout", "isActiveAndEnabled", "enabled" };

        public ComponentDescriptor(Type componentType)
        {
            componentName = componentType.Name;
            Properties = new List<PropertyDescriptor>();

            foreach (PropertyInfo propertyInfo in componentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                ReflectionInfoWrapper wrapper = new ReflectionInfoWrapper(propertyInfo);
                if (wrapper.CanRead && !exclusions.Contains(propertyInfo.Name))
                    Properties.Add(new PropertyDescriptor(wrapper));
            }
            foreach (FieldInfo fieldInfo in componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!exclusions.Contains(fieldInfo.Name))
                    Properties.Add(new PropertyDescriptor(new ReflectionInfoWrapper(fieldInfo)));
            }
        }

        public ComponentDescriptor(Type componentType, params string[] propertyNames)
        {
            componentName = componentType.Name;
            Properties = new List<PropertyDescriptor>();

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