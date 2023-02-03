using AttributeTargets = System.AttributeTargets;
using String = System.String;

namespace CrabGameUtils.Extensions;

[ExtensionName("Texture replacer")]
public class TextureReplacer : Extension
{
    public ExtensionConfig<string> Prefix = new("prefix", "!", "the prefix for commands related with this extension");

    private static System.Collections.Generic.Dictionary<string, TextureReplacerTexture>? _textures;
    private static TextureReplacerTexture? _current;
    private static TextureReplacerTexture? _default;

    public TextureReplacer()
    {
        Events.ChatBoxSubmitEvent += GetChatMessageLocal;
        _textures = new();
        
        foreach (Type texture in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TextureReplacerTexture))))
        {
            string name = texture.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.Name == "TextureNameAttribute")?.ConstructorArguments.First().Value?.ToString() ?? texture.Name;
            TextureReplacerTexture instance = (TextureReplacerTexture)System.Activator.CreateInstance(texture, null);
            if (!_textures!.ContainsKey(name.ToLower()))
                _textures.Add(name.ToLower(), instance);
            string trName = GetType().GetCustomAttributesData().First(a => a.AttributeType.Name == "ExtensionNameAttribute").ConstructorArguments.First().Value?.ToString() ?? GetType().Name;
            foreach (FieldInfo field in texture.GetFields().Where(p => p.FieldType.Name.Contains("ExtensionConfig")))
                field.GetValue(instance).GetType().GetMethod("InitConfig")!.Invoke(field.GetValue(instance), new object[] { $"{trName} | {name}" });
        }

        AddAutoCompletions();
    }

    private void AddAutoCompletions()
    {
        AutoComplete.AddCompletion("!textures");
        AutoComplete.AddCompletion("!textures help");
        for (int i = 1; i <= 2; i++)
        {
            AutoComplete.AddCompletion($"!textures help {i}");
        }
        AutoComplete.AddCompletion("!textures enable");
        foreach (System.Collections.Generic.KeyValuePair<string, TextureReplacerTexture> texture in _textures!)
        {
            AutoComplete.AddCompletion($"!textures enable {texture.Key}");
        }
        AutoComplete.AddCompletion("!textures disable");
        AutoComplete.AddCompletion("!textures setdefault");
        foreach (System.Collections.Generic.KeyValuePair<string, TextureReplacerTexture> texture in _textures)
        {
            AutoComplete.AddCompletion($"!textures setdefault {texture.Key}");
        }
        AutoComplete.AddCompletion("!textures cleardefault");
    }

    private void RemoveAutoCompletions()
    {
        AutoComplete.SetCompletions(AutoComplete.GetCompletions().Where(n => !n.StartsWith("!textures")).ToList());
    }
    
    public override void Awake()
    {
        string? defaultName = Configuration.GlobalTemplate.TextureReplacer!.DefaultName;
        if (defaultName != null && _textures!.ContainsKey(defaultName)) _default = _textures[defaultName];
    }

    public override void Start()
    {
        foreach (TextureReplacerTexture texture in _textures!.Values) texture.Start();
        
        if (_current != null) RemoveCurrent();
        if (_default != null) SetCurrent(_default);

        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Texture replacer loaded, type \"{Prefix.Value}textures help\" for help.</color>");
    }
    
    public override void Update()
    {
        try
        {
            _current?.Update();
        } catch { /**/ }
    }

    public void ProcessCommand(string[] args)
    {
        if (args.Length < 1)
        {
            ChatBox.Instance.ForceMessage(_textures!.Keys.Aggregate("<color=#00FFFF>--- Textures list ---</color>", (s, s1) => $"{s}\n<color=green>- {s1}</color>"));
            return;
        }
        
        switch (args[0].ToLower())
        {
            case "enable":
            {
                string textureName = String.Join(" ", args[1..]).ToLower();

                if (textureName.Equals("random"))
                {
                    if (_textures!.Count == 0)
                    {
                        ChatBox.Instance.ForceMessage("<color=red>Can't</color>");
                        return;
                    }
                    SetCurrent(_textures.Values.ElementAt(UnityEngine.Random.Range(0, _textures.Count)));
                    return;
                }
                
                if (!_textures!.ContainsKey(textureName))
                {
                    ChatBox.Instance.ForceMessage($"<color=red>Couldn't find any texture with \"{textureName}\"</color>");
                    return;
                }
                SetCurrent(_textures[textureName]);
                ChatBox.Instance.ForceMessage($"<color=green>{textureName} enabled.</color>");
            }
                break;
            case "disable":
            {
                if (_current == null)
                {
                    ChatBox.Instance.ForceMessage("<color=yellow>Nothing to disable.</color>");
                    return;
                }
                RemoveCurrent();
                ChatBox.Instance.ForceMessage("<color=green>Current theme disabled.</color>");
            }
                break;
            case "help":
                string[] pages =
                {
                    "!textures - Without arguments it will show you a list of available textures\n!textures enable name - Will enable \"name\" texture and disable current if there is.\n!textures disable - Will disable active texture if there is.".Replace("!", Prefix.Value),
                    "!textures setdefault texture - Will set the default texture to defined texture and when you start the game it will automatically enable\n!textures cleardefault - Will remove the default and disable it.".Replace("!", Prefix.Value)
                };
                int page;
                if (args.Length < 2)
                {
                    page = 1;
                }
                else if (!int.TryParse(args[1], out page) || page < 1 || page > pages.Length)
                {
                    ChatBox.Instance.ForceMessage("<color=yellow>not a valid page number.</color>");
                    return;
                }

                ChatBox.Instance.ForceMessage($"<color=#00FFFF>--- Textures help (page {page} out of {pages.Length}) ---</color>\n<color=green>{pages[page - 1]}</color>\n<color=#00FFFF>--- end ---</color>");
                break;
            case "setdefault":
            {
                string textureName = String.Join(" ", args[1..]).ToLower();

                if (string.IsNullOrEmpty(textureName) && _current != null)
                {
                    _default = _current;
                    ChatBox.Instance.ForceMessage("<color=green>Default texture set to current.</color>");
                }
                else if (!string.IsNullOrEmpty(textureName) && _textures!.ContainsKey(textureName))
                {
                    _default = _textures[textureName]!;
                    ChatBox.Instance.ForceMessage($"<color=green>Default texture set to {textureName}</color>");
                }
                else
                {
                    ChatBox.Instance.ForceMessage("<color=yellow>No texture to set, use a third argument to set a texture</color>");
                    return;
                }

                if (_current != null)
                    Disable(_current);
                
                _current = _default;
                Enable(_current);

                Configuration.GlobalTemplate.TextureReplacer!.DefaultName = textureName;
                Configuration.Write();
                
                ChatBox.Instance.ForceMessage("<color=yellow>(Beta feature) enabled only for this game instance, this warning and the limitation will disapear in the next version.</color>");
            }
                break;
            case "cleardefault":
                if (_default != null)
                {
                    RemoveCurrent();
                    _default = null;
                    ChatBox.Instance.ForceMessage("<color=green>Cleared default.</color>");
                    return;
                }
                
                ChatBox.Instance.ForceMessage("<color=yellow>Nothing to clear.</color>");
                break;
            default:
                ChatBox.Instance.ForceMessage($"<color=yellow>Could not find function {args[0]}</color>");
                break;
        }
    }

    public static void SetCurrent(TextureReplacerTexture texture)
    {
        if (_current != null) Disable(_current);
        _current = texture;
        Enable(_current);
    }
    
    public static void RemoveCurrent()
    {
        if (_current == null) return;
        Disable(_current);
        _current = null;
    }

    public static void Enable(TextureReplacerTexture texture)
    {
        texture.Enabled = true;
        texture.Enable();
    }

    public static void Disable(TextureReplacerTexture texture)
    {
        texture.Enabled = false;
        texture.Disable();
    }

    public void GetChatMessageLocal(string text)
    {
        string[] args = text.Split(" ");

        if (args[0].ToLower() != $"{Prefix.Value}textures") return;
        ProcessCommand(args[1..]);
    }
}

public abstract class TextureReplacerTexture
{
    public bool Enabled;
    
    public abstract void Start();
    public abstract void Update();
    public abstract void Enable();
    public abstract void Disable();
}

[System.AttributeUsage(AttributeTargets.Class)]
public class TextureNameAttribute : System.Attribute
{
    // ReSharper disable once UnusedParameter.Local
    public TextureNameAttribute(string name) {}
}