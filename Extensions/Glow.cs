using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

[ExtensionName("Player glow")]
public class Glow : Extension
{
    public ExtensionConfig<string> Key = new("key", "i", "the key to enable or disable ");
    
    private bool _enabled;
    private GameObject _sun = null!;
    private Color _defaultAmbient;
    private System.Collections.Generic.Dictionary<int, DefaultObjectColors> _defaultObjectColors = default!;
    private System.Collections.Generic.Dictionary<ulong, GameObject> _playerGameObjects = default!;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    public override void Start()
    {
        _defaultObjectColors = new();
        _playerGameObjects = new();
        if (!System.Enum.TryParse(Key.Value.ToUpper(), out KeyCode _))
        {
            ThrowError("Party errored, the keycode is not valid.");
            return;
        }

        _sun = GameObject.Find("Directional Light");
        _defaultAmbient = RenderSettings.ambientLight;

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
            foreach (Material rendererMaterial in renderer.materials)
                _defaultObjectColors.Add(rendererMaterial.GetInstanceID(), new DefaultObjectColors
                {
                    MaterialColor = rendererMaterial.color,
                    EmisionColor = rendererMaterial.GetColor(EmissionColor)
                });
            

        foreach (Light light in Object.FindObjectsOfType<Light>())
            _defaultObjectColors.Add(light.GetInstanceID(), new DefaultObjectColors{MaterialColor = light.color});

        if (_enabled)
        {
            _sun.active = !_enabled;
            RenderSettings.ambientLight = _enabled ? new Color(0.07f, 0.07f, 0.07f) : _defaultAmbient;
            foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
                foreach (Material rendererMaterial in renderer.materials)
                    rendererMaterial.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
        }

        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Party Loaded, press {Key.Value} to go on drugs.</color><color=orange>{(GameManager.Instance.activePlayers.count > 15 ? "(fps warning)" : "")}</color>");
    }

    private Color RotateColor(Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return Color.HSVToRGB(h + Time.deltaTime / 5 >= 1 ? 0 : h + Time.deltaTime / 5, 1, 1);
    }

    private void Disable()
    {
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            GameObject light = GameObject.Find($"Light-{player.Key}");
            if (light)
            {
                _playerGameObjects.Remove(player.Key);
                Object.Destroy(light);
            }
        }

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
            foreach (Material rendererMaterial in renderer.materials)
            {
                int id = rendererMaterial.GetInstanceID();
                if (!_defaultObjectColors.ContainsKey(id)) continue;
                if (rendererMaterial.color != _defaultObjectColors[id].MaterialColor) rendererMaterial.color = _defaultObjectColors[id].MaterialColor;
                rendererMaterial.SetColor(EmissionColor, _defaultObjectColors[rendererMaterial.GetInstanceID()].EmisionColor!.Value);
            }
        

        foreach (Light light in Object.FindObjectsOfType<Light>().Where(i => !i.name.StartsWith("Light-")))
            if (light.color != _defaultObjectColors[light.GetInstanceID()].MaterialColor) light.color = _defaultObjectColors[light.GetInstanceID()].MaterialColor;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(Key.Value.ToLower()) && !ChatBox.Instance.inputField.isFocused)
        {
            _enabled = !_enabled;
            _sun.active = !_enabled;
            RenderSettings.ambientLight = _enabled ? new Color(0.07f, 0.07f, 0.07f) : _defaultAmbient;
            if (!_enabled) Disable();
            else
            {
                foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
                    foreach (Material rendererMaterial in renderer.materials)
                        rendererMaterial.SetColor(Color1, Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1));
            }
            ChatBox.Instance.ForceMessage($"<color=orange>Party mode:</color> {(_enabled ? "<color=green>enabled</color>" : "<color=red>disabled</color>")}");
        }
        
        if (!_enabled) return;
        
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            GameObject light;
            
            if (!_playerGameObjects.ContainsKey(player.Key))
            {
                light = new GameObject($"Light-{player.Key}");
                _playerGameObjects.Add(player.Key, light);
            }
            else
                light = _playerGameObjects[player.Key];
            
            
            
            Light lightComp = light.GetComponent<Light>();

            if (!lightComp)
            {
                lightComp = light.AddComponent<Light>();
                lightComp.type = LightType.Point;
                lightComp.intensity = 4;
                lightComp.range = 30;
                lightComp.bounceIntensity = 1;
                lightComp.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
                lightComp.renderMode = LightRenderMode.ForcePixel;
            }
            
            lightComp.color = RotateColor(lightComp.color);
            light.transform.position = player.Value.transform.position + new Vector3(0, 2, 0);
        }

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetColor(Color1, RotateColor(rendererMaterial.color));
                rendererMaterial.SetColor(EmissionColor, rendererMaterial.color);
            }
        

        foreach (Light light in Object.FindObjectsOfType<Light>().Where(i => !i.name.StartsWith("Light-")))
            light.color = RotateColor(light.color);
    }

    private struct DefaultObjectColors
    {
        public Color MaterialColor { get; set; }
        public Color? EmisionColor { get; set; }
    }
}