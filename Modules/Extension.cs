
using Action = System.Action;

namespace CrabGameUtils.Modules;

public abstract class Extension
{
    public string Name = default!;
    public ExtensionConfig<bool> Enabled = new("toggle", true, "Whether to enable or disable the plugin");
    public abstract void Awake();
    public abstract void Start();
    public abstract void Update();
    
    protected void ThrowError(string message)
    {
        Enabled.Value = false;
        ChatBox.Instance.ForceMessage($"<color=red>{message}</color>");
    }

    protected void Disable()
    {
        Enabled.Value = false;
    }
}

public class ExtensionConfig<T>
{
    private readonly string _settingName;
    private readonly T _defaultValue;
    private readonly string _description;

    public T Value { get; set; } = default!;

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