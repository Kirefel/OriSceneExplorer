namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public abstract class PropertyEditor
    {
        public abstract bool Draw(ref object value);
        public virtual string FormatString(object value) => value == null ? "(null)" : value.ToString();
    }
}