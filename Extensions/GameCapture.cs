
namespace CrabGameUtils.Extensions;

[ExtensionName("Game capture")]
public class GameCapture : Extension
{
    public ExtensionConfig<string> Path = new("path", $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)}", "Where to save the image");
    public ExtensionConfig<string> Key = new("key", "O", "The key that should save the screenshot");

    public override void Start()
    {
        if (!System.Enum.TryParse(Key.Value, out KeyCode _))
        {
            ThrowError("GameCapture Error, the key is not valid.");
            return;
        }

        if (!Directory.Exists(Path.Value))
        {
            ThrowError("GameCapture Error, the path is not valid.");
            return;
        }
        
        ChatBox.Instance.ForceMessage($"<color=#00FFFF>GameCapture Loaded, press {Key.Value} to screenshot</color>");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(System.Enum.Parse<UnityEngine.KeyCode>(Key.Value)) && !ChatBox.Instance.inputField.isFocused)
        {
            Camera? camera = Camera.main;
            int resWidth = Screen.width, resHeight = Screen.height;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera!.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string curPath = System.IO.Path.Combine(Path.Value, $"{System.DateTime.Now:HHmmssyyyyMMdd}.png");
            File.WriteAllBytes(curPath, bytes);
            ChatBox.Instance.ForceMessage($"<color=#00FFFF>Screenshoot saved to:</color> <color=orange>\"{curPath}\"</color>");
        }
    }
}