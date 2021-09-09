namespace OriSceneExplorer.Inspector
{
    public class PropertyDescriptor
    {
        public string Name { get; }
        public string TypeName { get; }
        public ReflectionInfoWrapper Info { get; }

        public PropertyDescriptor(ReflectionInfoWrapper reflectionInfo)
        {
            Info = reflectionInfo;
            Name = reflectionInfo.Name;
            TypeName = reflectionInfo.TypeName;
        }
    }
}