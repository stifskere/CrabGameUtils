
namespace CrabGameUtils.Extensions;

[ExtensionName("Game capture")]
public class GameCapture : Extension
{
    public ExtensionConfig<string> Path = new("path", $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)}", "Where to save the image");
    public ExtensionConfig<string> Key = new("key", "o", "The key that should save the screenshot");

    private KeyCode _key;

    public override void Start()
    {
        if (!System.Enum.TryParse(Key.Value.Length == 1 ? Key.Value.ToUpper() : Key.Value, out _key))
        {
            ThrowError("GameCapture Errored, the key is not valid.");
            return;
        }

        if (!Directory.Exists(Path.Value))
        {
            ThrowError("GameCapture Errored, the path is not valid.");
            return;
        }
        
        ChatBox.Instance.ForceMessage($"GameCapture Loaded, press {Key.Value} to screenshot");
    }

    public override void Update()
    {
        //can't use a damn library
        if (Input.GetKeyDown((UnityEngine.KeyCode)_key))
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
            byte[] bytes = screenShot.GetRawTextureData();
            using Image<Rgb24> img = new(resWidth, resHeight);
            img.CopyPixelDataTo(new Span<byte>(bytes));
            string actualPath = $@"{Path.Value}\{System.DateTime.Now.ToString(CultureInfo.CurrentCulture)}.png";
            img.Save(actualPath, new PngEncoder());
            ChatBox.Instance.ForceMessage($"Image saved to: {actualPath}");
        }
    }
}