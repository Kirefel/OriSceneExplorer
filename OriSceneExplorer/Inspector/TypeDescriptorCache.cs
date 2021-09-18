using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public static class TypeDescriptorCache
    {
        private static Dictionary<Type, TypeDescriptor> descriptors = new Dictionary<Type, TypeDescriptor>();

        static TypeDescriptorCache()
        {
            descriptors[typeof(Transform)] = new TypeDescriptor(typeof(Transform), "position", "localPosition", "eulerAngles", "localScale");
        }

        public static TypeDescriptor GetDescriptor(Type type)
        {
            if (!descriptors.ContainsKey(type))
                descriptors[type] = new TypeDescriptor(type);
            return descriptors[type];
        }
    }
}