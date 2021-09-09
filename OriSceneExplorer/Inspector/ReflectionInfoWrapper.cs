using System;
using System.Linq;
using System.Reflection;

namespace OriSceneExplorer.Inspector
{
    public class ReflectionInfoWrapper
    {
        private readonly FieldInfo _fieldInfo;
        private readonly PropertyInfo _propertyInfo;

        public ReflectionInfoWrapper(FieldInfo fieldInfo) { _fieldInfo = fieldInfo; }
        public ReflectionInfoWrapper(PropertyInfo propertyInfo) { _propertyInfo = propertyInfo; }
        public ReflectionInfoWrapper(Type type, string name)
        {
            var propInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (type != null)
            {
                _propertyInfo = propInfo;
            }
            else
            {
                var fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    _fieldInfo = fieldInfo;
                }
                else
                {
                    throw new Exception($"{name} is not a field or property of {type.Name}");
                }
            }
        }

        public Type Type => _fieldInfo?.FieldType ?? _propertyInfo.PropertyType;
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
        public string Name => _fieldInfo?.Name ?? _propertyInfo.Name;
        public string TypeName => GetFormattedName(Type);

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