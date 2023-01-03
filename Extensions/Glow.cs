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

        foreach (MeshRenderer renderer in Object.FindObjectsOfType<MeshRenderer>())
        {
            _defaultObjectColors.Add(renderer.GetInstanceID(), new DefaultObjectColors
            {
                MaterialColor = renderer.material.color,
                EmisionColor = renderer.material.GetColor(EmissionColor)
            });
        }

        foreach (Light light in Object.FindObjectsOfType<Light>())
            _defaultObjectColors.Add(light.GetInstanceID(), new DefaultObjectColors{MaterialColor = light.color});

        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Party Loaded, press {Key.Value} to go on drugs.</color>");
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

        foreach (MeshRenderer renderer in Object.FindObjectsOfType<MeshRenderer>())
        {
            int id = renderer.GetInstanceID();
            if (!_defaultObjectColors.ContainsKey(id)) continue;
            if (renderer.material.color != _defaultObjectColors[id].MaterialColor) renderer.material.color = _defaultObjectColors[id].MaterialColor;
            renderer.material.SetColor(EmissionColor, _defaultObjectColors[renderer.GetInstanceID()].EmisionColor!.Value);
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
            {
                light = _playerGameObjects[player.Key];
            }
            
            
            Light lightComp = light.GetComponent<Light>();

            if (!lightComp)
            {
                lightComp = light.AddComponent<Light>();
                lightComp.type = LightType.Point;
                lightComp.intensity = 4;
                lightComp.range = 30;
                lightComp.bounceIntensity = 1;
                lightComp.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
            }
            
            lightComp.color = RotateColor(lightComp.color);
            light.transform.position = player.Value.transform.position + new Vector3(0, 2, 0);
        }

        foreach (MeshRenderer renderer in Object.FindObjectsOfType<MeshRenderer>())
        {
            Material material;
            (material = renderer.material).color = RotateColor(renderer.material.color);
            renderer.material.SetColor(EmissionColor, material.color);
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