using AttributeTargets = System.AttributeTargets;
using String = System.String;

namespace CrabGameUtils.Extensions;

[ExtensionName("Texture replacer")]
public class TextureReplacer : Extension
{
    protected static System.Collections.Generic.Dictionary<string, TextureReplacerTexture>? Textures;
    protected static TextureReplacerTexture? Current;

    public TextureReplacer()
    {
        Events.ChatBoxSubmitEvent += GetChatMessageLocal;
        
        Textures = new();

        foreach (Type texture in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TextureReplacerTexture))))
        {
            string name = texture.GetCustomAttributesData().First(a => a.AttributeType.Name == "TextureNameAttribute").ConstructorArguments.First().Value?.ToString() ?? texture.Name;
            TextureReplacerTexture instance = (TextureReplacerTexture)System.Activator.CreateInstance(texture, null);
            if (!Textures.ContainsKey(name.ToLower()))
                Textures.Add(name.ToLower(), instance);
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
        foreach (System.Collections.Generic.KeyValuePair<string, TextureReplacerTexture> texture in Textures!)
        {
            AutoComplete.AddCompletion($"!textures enable {texture.Key}");
        }
        AutoComplete.AddCompletion("!textures disable");
        AutoComplete.AddCompletion("!textures setdefault");
        foreach (System.Collections.Generic.KeyValuePair<string, TextureReplacerTexture> texture in Textures!)
        {
            AutoComplete.AddCompletion($"!textures setdefault {texture.Key}");
        }
        AutoComplete.AddCompletion("!textures cleardefault");
    }
    
    public override void Awake()
    {
        
    }

    public override void Start()
    {
        if (Current != null) 
        {
            Disable(Current);
            Current = null;
        }

        foreach (TextureReplacerTexture texture in Textures!.Values) texture.Start();
        ChatBox.Instance.ForceMessage("<color=#00FFFF>Texture replacer loaded, type \"!textures help\" for help.</color>");
    }
    
    public override void Update()
    {
        Current?.Update();
    }

    public static void ProcessCommand(string[] args)
    {
        if (args.Length < 1)
        {
            ChatBox.Instance.ForceMessage(Textures!.Keys.Aggregate("<color=#00FFFF>--- Textures list ---</color>", (s, s1) => $"{s}\n<color=green>- {s1}</color>"));
            return;
        }
        
        switch (args[0].ToLower())
        {
            case "enable":
            {
                string textureName = String.Join(" ", args[1..]).ToLower();

                if (textureName.Equals("random"))
                {
                    if (Textures!.Count == 0)
                    {
                        ChatBox.Instance.ForceMessage("<color=red>Can't</color>");
                        return;
                    }
                    SetCurrent(Textures!.Values.ElementAt(UnityEngine.Random.Range(0, Textures.Count)));
                    return;
                }
                
                if (!Textures!.ContainsKey(textureName))
                {
                    ChatBox.Instance.ForceMessage($"<color=red>Couldn't find any texture with \"{textureName}\"</color>");
                    return;
                }
                SetCurrent(Textures[textureName]);
                ChatBox.Instance.ForceMessage($"<color=green>{textureName} enabled.</color>");
            }
                break;
            case "disable":
            {
                if (Current == null)
                {
                    ChatBox.Instance.ForceMessage("<color=yellow>Nothing to disable.</color>");
                    return;
                }
                RemoveCurrent();
                ChatBox.Instance.ForceMessage("<color=green>Current theme disabled.</color>");
            }
                break;
            case "help":
                ChatBox.Instance.ForceMessage("<color=#00FFFF>--- Textures help ---</color>\n<color=green>!textures - Without arguments it will show you a list of available textures\n!textures enable name - Will enable \"name\" texture and disable current if there is.\n!textures disable - Will disable active texture if there is</color>.\n<color=#00FFFF>--- end ---</color>");
                break;
            default:
                ChatBox.Instance.ForceMessage($"<color=yellow>Could not find function {args[0]}</color>");
                break;
        }
    }

    public static void SetCurrent(TextureReplacerTexture texture)
    {
        if (Current != null) Disable(Current);
        Current = texture;
        Enable(Current);
    }
    
    public static void RemoveCurrent()
    {
        Disable(Current!);
        Current = null;
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

    public static void GetChatMessageLocal(string text)
    {
        string[] args = text.Split(" ");

        if (args[0].ToLower() != "!textures") return;
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