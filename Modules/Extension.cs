namespace CrabGameUtils.Modules;

public abstract class Extension
{
    public abstract void Start();
    public abstract void Update();
}

public class ExtensionConfig<T>
{
    private readonly string _settingName;
    private readonly T _defaultValue;
    private readonly string _description;

    public T Value { get; protected set; } = default!;
    
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