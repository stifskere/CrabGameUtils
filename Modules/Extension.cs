
namespace CrabGameUtils.Modules;

public abstract class Extension
{
    public ExtensionConfig<bool> Enabled = new("toggle", true, "Whether to enable or disable the plugin");
    public abstract void Start();
    public abstract void Update();

    public void ThrowError(string message)
    {
        Enabled.Value = false;
        ChatBox.Instance.ForceMessage($"<color=red>{message}</color>");
    }
}

public class ExtensionConfig<T>
{
    private readonly string _settingName;
    private readonly T _defaultValue;
    private readonly string _description;

    private T _actual = default!;
    public T Value { get => _actual; set => _actual = value; }
    
    public ExtensionConfig(string settingName, T defaultValue, string description)
    {
        _settingName = settingName;
        _defaultValue = defaultValue;
        _description = description;
    }

    public void InitConfig(string methodName)
    {
        Value = StaticConfig.Bind(methodName, _settingName, _defaultValue, _description).Value;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class ExtensionNameAttribute : System.Attribute
{
    public ExtensionNameAttribute(string name) { }
}