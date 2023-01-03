using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

[ExtensionName("Night mode")]
public class NightMode : Extension
{
    public ExtensionConfig<string> Key = new("key", "N", "the key to enable or disable");
    
    private bool _enabled;
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private Color _skyboxColor;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public override void Start()
    {
        _materials = new();
        if (!System.Enum.TryParse(Key.Value, out KeyCode _))
        {
            ThrowError("Party errored, the keycode is not valid.");
            return;
        }
        _skyboxColor = RenderSettings.skybox.color;
        
        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Night mode Loaded, press {Key.Value}.</color><color=orange>");
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(System.Enum.Parse<UnityEngine.KeyCode>(Key.Value)) && !ChatBox.Instance.inputField.isFocused)
        {
            _enabled = !_enabled;
            if (!_enabled)
            {
                foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
                {
                    if (!_materials.ContainsKey(renderer.GetInstanceID())) continue;
                    renderer.materials = _materials[renderer.GetInstanceID()].ToArray();
                }
                RenderSettings.skybox.color = _skyboxColor;
            }
            else
            {
                _skyboxColor = RenderSettings.skybox.color;
                RenderSettings.skybox.color = Color.black;
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