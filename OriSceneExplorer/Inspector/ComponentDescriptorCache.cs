using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public static class ComponentDescriptorCache
    {
        private static Dictionary<Type, ComponentDescriptor> descriptors = new Dictionary<Type, ComponentDescriptor>();

        static ComponentDescriptorCache()
        {
            descriptors[typeof(Transform)] = new ComponentDescriptor(typeof(Transform), "position", "localPosition", "eulerAngles");
        }

        public static ComponentDescriptor GetDescriptor(Type componentType)
        {
            if (!descriptors.ContainsKey(componentType))
                descriptors[componentType] = new ComponentDescriptor(componentType);
            return descriptors[componentType];
        }
    }
}