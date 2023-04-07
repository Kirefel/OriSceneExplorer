using UnityEngine;

namespace OriSceneExplorer.Inspector
{
    public class MethodInspector
    {
        public MethodDescriptor MethodDescriptor { get; set; }

        public MethodInspector(MethodDescriptor methodDescriptor, object instance)
        {
            MethodDescriptor = methodDescriptor;
        }

        public void Draw(bool editable, object instance)
        {
            if (GUILayout.Button(MethodDescriptor.Name))
            {
                if (MethodDescriptor.Method.IsStatic)
                    MethodDescriptor.Method.Invoke(null, null);
                else if (editable)
                    MethodDescriptor.Method.Invoke(instance, null);
            }
        }
    }
}