using OriSceneExplorer.Inspector.PropertyEditors;
using System;
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

        public static PropertyEditor CreateEditor(PropertyDescriptor descriptor)
        {
            if (editors.ContainsKey(descriptor.Info.Type))
                return editors[descriptor.Info.Type]();

            if (descriptor.Info.Type.IsEnum)
                return new EnumEditor(descriptor.Info.Type);

            if (typeof(Component).IsAssignableFrom(descriptor.Info.Type))
                return new ComponentEditor();

            if (typeof(GameObject).IsAssignableFrom(descriptor.Info.Type))
                return new GameObjectEditor();

            return new DefaultEditor();
        }
    }
}