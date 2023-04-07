namespace OriSceneExplorer.Inspector
{
    public class TypeInspector
    {
        private readonly PropertyInspector[] propertyInspectors;
        private readonly MethodInspector[] methodInspectors;

        public TypeInspector(TypeDescriptor typeDescriptor, object objectInstance)
        {
            propertyInspectors = new PropertyInspector[typeDescriptor.Properties.Count];
            for (int i = 0; i < typeDescriptor.Properties.Count; i++)
                propertyInspectors[i] = new PropertyInspector(typeDescriptor.Properties[i], objectInstance);

            methodInspectors = new MethodInspector[typeDescriptor.Methods.Count];
            for (int i = 0; i < typeDescriptor.Methods.Count; i++)
                methodInspectors[i] = new MethodInspector(typeDescriptor.Methods[i], objectInstance);
        }

        public void Draw(bool editable, object objectInstance)
        {
            for (int i = 0; i < propertyInspectors.Length; i++)
                propertyInspectors[i].Draw(editable, objectInstance);
            for (int i = 0; i < methodInspectors.Length; i++)
                methodInspectors[i].Draw(editable, objectInstance);
        }
    }
}