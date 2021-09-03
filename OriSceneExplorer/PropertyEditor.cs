using OriSceneExplorer.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OriSceneExplorer
{
    public class PropertyEditorFactory
    {
        delegate IPropertyEditor CreateEditorFunc(ReflectionInfoWrapper wrapper, object instance);

        private readonly Dictionary<Type, CreateEditorFunc> editors;

        public PropertyEditorFactory()
        {
            editors = new Dictionary<Type, CreateEditorFunc>();
            var propEditors = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(PropertyEditorAttribute), false).Length > 0);
            foreach (var peType in propEditors)
            {
                var attribute = peType.GetCustomAttributes(typeof(PropertyEditorAttribute), false).First() as PropertyEditorAttribute;
                var targetType = attribute.Type;

                editors[targetType] = (ReflectionInfoWrapper wrapper, object instance) =>
                    (IPropertyEditor)Activator.CreateInstance(peType, wrapper, instance);
            }
        }

        public IPropertyEditor CreateEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            if (editors.ContainsKey(wrapper.MemberType))
                return editors[wrapper.MemberType](wrapper, instance);

            if (wrapper.MemberType.IsEnum)
                return new EnumEditor(wrapper, instance);

            return new DefaultEditor(wrapper, instance);
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PropertyEditorAttribute : Attribute
    {
        public PropertyEditorAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }


    [PropertyEditor(typeof(bool))]
    public class BoolEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly object _instance;

        private bool value;

        public BoolEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            _instance = instance;
            value = (bool)wrapper.GetValue(instance);
        }

        public void Draw()
        {
            bool newValue = GUILayout.Toggle(value, value.ToString());

            if (value != newValue)
            {
                value = newValue;
                Target.SetValue(_instance, value);
            }
        }

        public string StringValue() => value.ToString();
    }

    [PropertyEditor(typeof(string))]
    public class StringEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly object _instance;

        private string value;

        public StringEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            _instance = instance;
            value = (string)wrapper.GetValue(instance);
        }

        public void Draw()
        {
            string newValue = GUILayout.TextField(value);

            if (value != newValue)
            {
                value = newValue;
                Target.SetValue(_instance, value);
            }
        }

        public string StringValue() => value.ToString();
    }

    [PropertyEditor(typeof(int))]
    public class IntEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly object _instance;

        private IntField intField;
        int value;

        public IntEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            _instance = instance;
            value = (int)wrapper.GetValue(instance);
            intField = new IntField(value);
        }

        public void Draw()
        {
            int newValue = intField.Draw(value);

            if (value != newValue)
            {
                value = newValue;
                Target.SetValue(_instance, value);
            }
        }

        public string StringValue() => value.ToString();
    }

    [PropertyEditor(typeof(float))]
    public class FloatEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly object _instance;

        private FloatField floatField;
        float value;

        public FloatEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            _instance = instance;
            value = (float)wrapper.GetValue(instance);
            floatField = new FloatField(value);
        }

        public void Draw()
        {
            float newValue = floatField.Draw(value);

            if (value != newValue)
            {
                value = newValue;
                Target.SetValue(_instance, value);
            }
        }

        public string StringValue() => value.ToString();
    }

    [PropertyEditor(typeof(Vector3))]
    public class Vector3Editor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly object _instance;

        private Vector3Field vector3Field;
        Vector3 value;

        public Vector3Editor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            _instance = instance;
            value = (Vector3)wrapper.GetValue(instance);
            vector3Field = new Vector3Field(value);
        }

        public void Draw()
        {
            Vector3 newValue = vector3Field.Draw(value);

            if (value != newValue)
            {
                value = newValue;
                Target.SetValue(_instance, value);
            }
        }

        public string StringValue() => value.ToString();
    }

    public class DefaultEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private readonly string value;

        public DefaultEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            value = wrapper.GetValue(instance).ToString();
        }

        public void Draw()
        {
            GUILayout.Label(value);
        }

        public string StringValue() => value;
    }

    public class ConstantEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target => null;

        private string label;
        public ConstantEditor(string value)
        {
            this.label = value;
        }

        public void Draw()
        {
            GUILayout.Label(label);
        }

        public string StringValue() => label;
    }

    public class EnumEditor : IPropertyEditor
    {
        public ReflectionInfoWrapper Target { get; }
        private string value;
        private readonly object instance;

        public EnumEditor(ReflectionInfoWrapper wrapper, object instance)
        {
            Target = wrapper;
            this.instance = instance;
            value = wrapper.GetValue(instance).ToString();
        }

        public void Draw()
        {
            if (GUILayout.Button(value + " (enum)", "Label"))
            {
                var enumNames = Enum.GetNames(Target.MemberType);
                ContextMenu.Show(ChooseEnumCallback, enumNames);
            }
        }

        public void ChooseEnumCallback(int index)
        {
            if (index >= 0)
            {
                var enumValues = Enum.GetValues(Target.MemberType);
                var chosenValue = enumValues.GetValue(index);
                Target.SetValue(instance, chosenValue);
                value = chosenValue.ToString();
            }
        }

        public string StringValue() => value;
    }

    public interface IPropertyEditor
    {
        ReflectionInfoWrapper Target { get; }
        void Draw();
        string StringValue();
    }

    public class Vector3Field
    {
        FloatField x, y, z;
        public Vector3Field(Vector3 value)
        {
            x = new FloatField(value.x);
            y = new FloatField(value.y);
            z = new FloatField(value.z);
        }

        public Vector3 Draw(Vector3 value)
        {
            Vector3 v;
            v.x = x.Draw(value.x);
            v.y = y.Draw(value.y);
            v.z = z.Draw(value.z);
            return v;
        }
    }

    public class IntField
    {
        private string stringValue;
        private bool valid = true;
        public IntField(int value)
        {
            stringValue = value.ToString();
        }

        public int Draw(int value, params GUILayoutOption[] options)
        {
            Color c = GUI.backgroundColor;
            if (!valid)
                GUI.backgroundColor = Color.red;

            int retVal = value;
            stringValue = GUILayout.TextField(stringValue, options);

            if (int.TryParse(stringValue, out int i))
            {
                retVal = i;
                valid = true;
            }
            else
            {
                valid = false;
            }

            GUI.backgroundColor = c;
            return retVal;
        }
    }

    public class FloatField
    {
        private string stringValue;
        private bool valid = true;
        public FloatField(float value)
        {
            stringValue = value.ToString();
        }

        public float Draw(float value, params GUILayoutOption[] options)
        {
            Color c = GUI.backgroundColor;
            if (!valid)
                GUI.backgroundColor = Color.red;

            float retVal = value;
            stringValue = GUILayout.TextField(stringValue, options);

            if (float.TryParse(stringValue, out float i))
            {
                retVal = i;
                valid = true;
            }
            else
            {
                valid = false;
            }

            GUI.backgroundColor = c;
            return retVal;
        }
    }
}
