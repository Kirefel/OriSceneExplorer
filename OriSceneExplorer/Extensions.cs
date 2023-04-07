using System;
using System.Reflection;

namespace OriSceneExplorer
{
    public static class Extensions
    {
        public static bool GetCustomAttribute<T>(this MethodInfo method, bool inherit, out T a) where T : Attribute
        {
            var atts = method.GetCustomAttributes(typeof(T), inherit);
            if (atts.Length > 0)
            {
                a = atts[0] as T;
                return true;
            }

            a = null;
            return false;
        }
    }
}