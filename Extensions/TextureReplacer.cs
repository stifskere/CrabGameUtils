using AttributeTargets = System.AttributeTargets;
using String = System.String;

namespace CrabGameUtils.Extensions;

[ExtensionName("Texture replacer")]
public class TextureReplacer : Extension
{
    protected static System.Collections.Generic.Dictionary<string, TextureReplacerTexture> Textures;
    protected static TextureReplacerTexture? Current;

    public override void Awake()
    {
        Textures = new();
        Events.ChatBoxSubmitEvent += GetChatMessageLocal;

        foreach (Type texture in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TextureReplacerTexture))))
        {
            string name = texture.GetCustomAttributesData().First(a => a.AttributeType.Name == "TextureNameAttribute").ConstructorArguments.First().Value?.ToString() ?? texture.Name;
            TextureReplacerTexture instance = (TextureReplacerTexture)System.Activator.CreateInstance(texture, null);
            Textures.Add(name.ToLower(), instance);
        }
    }

    public override void Start()
    {
        foreach (TextureReplacerTexture texture in Textures.Values) texture.Start(); 
        ChatBox.Instance.ForceMessage("<color=#00FFFF>Texture replacer loaded, type \"!textures help\" for help.</color>");
    }
    
    public override void Update()
    {
        Current?.Update();
    }

    public static void GetChatMessageLocal(string text)
    {
        string[] args = text.Split(" ");

        if (args[0].ToLower() != "!textures") return;

        if (args.Length <= 1)
        {
            ChatBox.Instance.ForceMessage(Textures.Keys.Aggregate("<color=#00FFFF>--- Textures list ---</color>", (s, s1) => $"{s}\n<color=green>- {s1}</color>"));
            return;
        }
        
        switch (args[1].ToLower())
        {
            case "enable":
            {
                string textureName = String.Join(" ", args[2..]).ToLower();
                if (!Textures.ContainsKey(textureName))
                {
                    ChatBox.Instance.ForceMessage($"<color=red>Couldn't find any texture with \"{textureName}\"</color>");
                    return;
                }

                if (Current != null)
                {
                    Current.Enabled = false; 
                    Current.Disable();
                }
                Current = Textures[textureName];
                Current.Enabled = true;
                Current.Enable();
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
                Current.Enabled = false;
                Current.Disable();
                Current = null;
                ChatBox.Instance.ForceMessage("<color=green>Current theme disabled.</color>");
            }
                break;
            
            case "help":
                ChatBox.Instance.ForceMessage("<color=#00FFFF>--- Textures help ---</color>\n<color=green>!textures - Without arguments it will show you a list of available textures\n!textures enable name - Will enable \"name\" texture and disable current if there is.\n!textures disable - Will disable active texture if there is</color>.\n<color=#00FFFF>--- end ---</color>");
                break;
        }
    }
}

public abstract class TextureReplacerTexture
{
    
    public bool Enabled { get; set; }
    
    public abstract void Start();
    public abstract void Update();
    public abstract void Enable();
    public abstract void Disable();
}

[System.AttributeUsage(AttributeTargets.Class)]
public class TextureNameAttribute : System.Attribute
{
    public TextureNameAttribute(string name) {}
}