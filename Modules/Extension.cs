using System;
using Attribute = Il2CppSystem.Attribute;
using AttributeTargets = Il2CppSystem.AttributeTargets;

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
public class ExtensionNameAttribute : Attribute
{
    public string Name;
    public ExtensionNameAttribute(string name)
    {
        Name = name;
    }
}