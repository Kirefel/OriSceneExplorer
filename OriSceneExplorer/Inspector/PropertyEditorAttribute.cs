using System;

namespace OriSceneExplorer.Inspector
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PropertyEditorAttribute : Attribute
    {
        public Type Type { get; }

        public PropertyEditorAttribute(Type type)
        {
            Type = type;
        }
    }
}
