namespace OriSceneExplorer.Inspector
{
    public class TypeInspector
    {
        private readonly PropertyInspector[] propertyInspectors;

        public TypeInspector(TypeDescriptor componentDescriptor, object objectInstance)
        {
            propertyInspectors = new PropertyInspector[componentDescriptor.Properties.Count];
            for (int i = 0; i < componentDescriptor.Properties.Count; i++)
                propertyInspectors[i] = new PropertyInspector(componentDescriptor.Properties[i], objectInstance);
        }

        public void Draw(bool editable, object objectInstance)
        {
            for (int i = 0; i < propertyInspectors.Length; i++)
                propertyInspectors[i].Draw(editable, objectInstance);
        }
    }
}