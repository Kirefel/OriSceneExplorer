using OriSceneExplorer.Inspector.PropertyEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class PropertyEditorFactory
    {
        delegate PropertyEditor CreateEditorFunc();

        private static readonly Dictionary<Type, CreateEditorFunc> editors;

        static PropertyEditorFactory()
        {
            editors = new Dictionary<Type, CreateEditorFunc>();
            var propEditors = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(PropertyEditorAttribute), false).Length > 0);
            foreach (var peType in propEditors)
            {
                var attribute = peType.GetCustomAttributes(typeof(PropertyEditorAttribute), false).First() as PropertyEditorAttribute;
                var targetType = attribute.Type;

                editors[targetType] = () => (PropertyEditor)Activator.CreateInstance(peType);
            }
        }

        public static PropertyEditor CreateEditor(PropertyDescriptor descriptor) => CreateEditor(descriptor.Info.Type);

        public static PropertyEditor CreateEditor(Type type)
        {
            if (editors.ContainsKey(type))
                return editors[type]();

            if (type.IsEnum)
                return new EnumEditor(type);

            if (typeof(Component).IsAssignableFrom(type))
                return new ComponentEditor(type);

            if (typeof(GameObject).IsAssignableFrom(type))
                return new GameObjectEditor();

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return new EnumerableEditor(type);

            return new DefaultEditor(type);
        }
    }
}