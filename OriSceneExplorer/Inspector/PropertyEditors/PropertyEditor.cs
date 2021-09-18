namespace OriSceneExplorer.Inspector.PropertyEditors
{
    public abstract class PropertyEditor
    {
        public bool CanExpand { get; }

        private bool expanded = false;
        public bool Expanded
        {
            get => expanded && CanExpand;
            set => expanded = value && CanExpand;
        }
        public bool DrawWhenReadonly { get; } = false;

        public PropertyEditor() : this(false, false) { }
        public PropertyEditor(bool canExpand, bool drawWhenReadonly)
        {
            CanExpand = canExpand;
            DrawWhenReadonly = drawWhenReadonly;
        }

        public abstract bool Draw(ref object value);
        public virtual string FormatString(object value) => value == null ? "(null)" : value.ToString();
    }
}
