using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

[ExtensionName("Player glow")]
public class Glow : Extension
{
    public ExtensionConfig<string> Key = new("key", "I", "the key to enable or disable");
    
    private bool _enabled;
    private GameObject _sun = null!;
    private Color _defaultAmbient;
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private System.Collections.Generic.Dictionary<int, Color> _lightColors = default!;
    private System.Collections.Generic.Dictionary<ulong, GameObject> _playerGameObjects = default!;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    public override void Awake()
    {
        
    }

    public override void Start()
    {
        _enabled = false;
        if (_materials != default!)
        {
            foreach (Material material in _materials.SelectMany(keyValuePair => keyValuePair.Value))
            {
                Object.Destroy(material);
            }
        }

        _materials = new();
        _lightColors = new();
        _playerGameObjects = new();
        
        if (!System.Enum.TryParse(Key.Value, out KeyCode _))
        {
            ThrowError("Player glow errored, the keycode is not valid.");
            return;
        }

        _sun = GameObject.Find("Directional Light");
        _defaultAmbient = RenderSettings.ambientLight;
        
        foreach (Light light in Object.FindObjectsOfType<Light>())
            _lightColors.Add(light.GetInstanceID(), light.color);

        if (_enabled)
        {
            _sun.active = !_enabled;
            RenderSettings.ambientLight = _enabled ? new Color(0.07f, 0.07f, 0.07f) : _defaultAmbient;
            Enable();
        }

        void OnEventsOnRemovePlayerEvent(ulong id)
        {
            if (!_playerGameObjects.ContainsKey(id)) return;
            Object.Destroy(_playerGameObjects[id]);
            _playerGameObjects.Remove(id);
        }

        Events.RemovePlayerEvent += OnEventsOnRemovePlayerEvent;
        Events.PlayerDiedEvent += (player, killer, idk) =>
        {
            if (!_playerGameObjects.ContainsKey(player)) return;
            Object.Destroy(_playerGameObjects[player]);
            _playerGameObjects.Remove(player);
        };
        
        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Player glow Loaded, press {Key.Value} to go on drugs.</color><color=orange>{(GameManager.Instance.activePlayers.count + GameManager.Instance.spectators.count > 15 ? "(fps warning)" : "")}</color>");
    }

    private Color RotateColor(Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return Color.HSVToRGB(h + Time.deltaTime / 5 >= 1 ? 0 : h + Time.deltaTime / 5, 1, 1);
    }
    
    public override void Update()
    {
        if (Input.GetKeyDown(System.Enum.Parse<UnityEngine.KeyCode>(Key.Value)) && !ChatBox.Instance.inputField.isFocused)
        {
            _enabled = !_enabled;
            _sun.active = !_enabled;
            RenderSettings.ambientLight = _enabled ? new Color(0.07f, 0.07f, 0.07f) : _defaultAmbient;
            if (!_enabled) Disable();
            else Enable();
        }
        
        if (!_enabled) return;
        
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            if (player.Value.dead) continue;
            GameObject light;
            
            if (!_playerGameObjects.ContainsKey(player.Key))
            {
                light = new GameObject($"Light-{player.Key}");
                _playerGameObjects.Add(player.Key, light);
                Instance.Log.LogInfo($"Player {player.value.username} added with id: {player.Key}");
            }
            else light = _playerGameObjects[player.Key];

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

        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.spectators)
        {
            GameObject light = GameObject.Find($"Light-{player.key}");
            if (light) Object.Destroy(light);
        }

        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetColor(Color1, RotateColor(rendererMaterial.color));
                rendererMaterial.SetColor(EmissionColor, rendererMaterial.color);
            }
        }

        foreach (Light light in Object.FindObjectsOfType<Light>().Where(i => !i.name.StartsWith("Light-")))
            light.color = RotateColor(light.color);
    }
    
    private void Enable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (_materials.ContainsKey(renderer.GetInstanceID())) continue;
            _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
        }
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        foreach (Material rendererMaterial in renderer.materials)
            rendererMaterial.SetColor(Color1, Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1));
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
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) continue;
            foreach (Material material in renderer.materials) Object.Destroy(material);
            renderer.materials = _materials[renderer.GetInstanceID()].ToArray();
        }
        

        foreach (Light light in Object.FindObjectsOfType<Light>().Where(i => !i.name.StartsWith("Light-")))
            if (light.color != _lightColors[light.GetInstanceID()]) light.color = _lightColors[light.GetInstanceID()];
    }
}