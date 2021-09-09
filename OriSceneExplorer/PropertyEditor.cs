namespace OriSceneExplorer
{
    //public class EnumEditor : IPropertyEditor
    //{
    //    public ReflectionInfoWrapper Target { get; }
    //    private string value;
    //    private readonly object instance;

    //    public EnumEditor(ReflectionInfoWrapper wrapper, object instance)
    //    {
    //        Target = wrapper;
    //        this.instance = instance;
    //        value = wrapper.GetValue(instance).ToString();
    //    }

    //    public void Draw()
    //    {
    //        if (GUILayout.Button(value + " (enum)", "Label"))
    //        {
    //            var enumNames = Enum.GetNames(Target.Type);
    //            ContextMenu.Show(ChooseEnumCallback, enumNames);
    //        }
    //    }

    //    public void ChooseEnumCallback(int index)
    //    {
    //        if (index >= 0)
    //        {
    //            var enumValues = Enum.GetValues(Target.Type);
    //            var chosenValue = enumValues.GetValue(index);
    //            Target.SetValue(instance, chosenValue);
    //            value = chosenValue.ToString();
    //        }
    //    }

    //    public string StringValue() => value;
    //}

    //public interface IPropertyEditor
    //{
    //    ReflectionInfoWrapper Target { get; }
    //    void Draw();
    //    string StringValue();
    //}
}
