// namespace CrabGameUtils.Extensions;
//
// [ExtensionName("Texture replacer")]
// public class TextureReplacer : Extension
// {
//     public override void Start()
//     {
//         Events.ChatBoxSubmitEvent += GetChatMessageLocal;
//     }
//
//     public static void GetChatMessageLocal(string text)
//     {
//         string[] args = text.Split(" ");
//         
//     }
//
//     public override void Update()
//     {
//         
//     }
// }
//
// public abstract class TextureReplacerTexture
// {
//     public abstract string TextureName { get; set; }
//     public bool Enabled { get; set; } = false;
//
//     public abstract void Start();
//     public abstract void Update();
//     public abstract void Disable();
// }