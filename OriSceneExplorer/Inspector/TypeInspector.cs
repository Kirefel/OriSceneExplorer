namespace OriSceneExplorer.Inspector
{
    public class TypeInspector
    {
        private readonly PropertyInspector[] propertyInspectors;

        public TypeInspector(TypeDescriptor typeDescriptor, object objectInstance)
        {
            propertyInspectors = new PropertyInspector[typeDescriptor.Properties.Count];
            for (int i = 0; i < typeDescriptor.Properties.Count; i++)
                propertyInspectors[i] = new PropertyInspector(typeDescriptor.Properties[i], objectInstance);
        }

        public void Draw(bool editable, object objectInstance)
        {
            for (int i = 0; i < propertyInspectors.Length; i++)
                propertyInspectors[i].Draw(editable, objectInstance);
        }
    }
}