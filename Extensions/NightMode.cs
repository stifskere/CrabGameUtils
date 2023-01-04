using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

[ExtensionName("Night mode")]
public class NightMode : Extension
{
    public ExtensionConfig<string> Key = new("key", "N", "the key to enable or disable");
    
    private bool _enabled;
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public override void Awake()
    {
        _enabled = false;
        if (!System.Enum.TryParse(Key.Value, out KeyCode _))
        {
            ThrowError("Night mode errored, the keycode is not valid.");
        }
    }

    public override void Start()
    {
        if (_materials != default!)
        {
            foreach (Material material in _materials.SelectMany(keyValuePair => keyValuePair.Value))
            {
                Object.Destroy(material);
            }
        }
        _materials = new();

        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Night mode Loaded, press {Key.Value}.</color><color=orange>");
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(System.Enum.Parse<UnityEngine.KeyCode>(Key.Value)) && !ChatBox.Instance.inputField.isFocused)
        {
            _enabled = !_enabled;
            if (!_enabled)
            {
                RenderSettings.sun.color = Color.white;
                foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
                {
                    if (!_materials.ContainsKey(renderer.GetInstanceID())) continue;
                    foreach (Material material in renderer.materials) Object.Destroy(material);
                    renderer.materials = _materials[renderer.GetInstanceID()].ToArray();
                }
                
                Light light = Camera.main!.GetComponent<Light>();
                Object.Destroy(light);
            }
            else
            {
                Light light = Camera.main!.gameObject.AddComponent<Light>();
                light.renderMode = LightRenderMode.ForcePixel;
                light.type = LightType.Spot;
                light.intensity = 10;
                light.range = 50;
                light.spotAngle = 90;
                RenderSettings.sun.color = Color.black;
            }
        }

        if (!_enabled) return;
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetColor(Color1, Color.black);
                rendererMaterial.SetColor(EmissionColor, Color.black);
            }
        }
    }
}