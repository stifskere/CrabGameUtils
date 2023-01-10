namespace CrabGameUtils.Extensions.TexturePacks;
using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

[TextureName("Rainbow")]
public class Rainbow : TextureReplacerTexture
{
    public ExtensionConfig<bool> lightsEnabled = new("lights enabled", false, "Defines if the plugin turns off the lights when it is enabled.");

    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private System.Collections.Generic.Dictionary<int, Color> _lightColors = default!;
    private System.Collections.Generic.Dictionary<ulong, GameObject> _playerGameObjects = default!;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private Color _ambientLight;
    
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
        _lightColors = new();
        _playerGameObjects = new();

        foreach (Light light in Object.FindObjectsOfType<Light>())
            _lightColors.Add(light.GetInstanceID(), light.color);

        void OnEventsOnRemovePlayerEvent(ulong id)
        {
            if (!_playerGameObjects.ContainsKey(id)) return;
            Object.Destroy(_playerGameObjects[id]);
            _playerGameObjects.Remove(id);
        }

        Events.RemovePlayerEvent += OnEventsOnRemovePlayerEvent;
        Events.PlayerDiedEvent += (player, _, _) =>
        {
            if (!_playerGameObjects.ContainsKey(player)) return;
            Object.Destroy(_playerGameObjects[player]);
            _playerGameObjects.Remove(player);
        };

        _ambientLight = RenderSettings.ambientLight;
    }

    public override void Update()
    {
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            if (player.Value.dead) continue;
            GameObject light;
            
            if (!_playerGameObjects.ContainsKey(player.Key))
            {
                light = new GameObject($"Light-{player.Key}");
                _playerGameObjects.Add(player.Key, light);
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

    public override void Enable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (_materials.ContainsKey(renderer.GetInstanceID())) continue;
            _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
        }
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        foreach (Material rendererMaterial in renderer.materials)
            rendererMaterial.SetColor(Color1, Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1));
        if (!lightsEnabled.Value)
            RenderSettings.ambientLight = new Color(0.07f, 0.07f, 0.07f);
    }

    public override void Disable()
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
        {
            if (!_lightColors.ContainsKey(light.GetInstanceID())) continue;
            if (light.color != _lightColors[light.GetInstanceID()]) light.color = _lightColors[light.GetInstanceID()];
        }
        
        RenderSettings.ambientLight = _ambientLight;
    }
    
    private Color RotateColor(Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return Color.HSVToRGB(h + Time.deltaTime / 5 >= 1 ? 0 : h + Time.deltaTime / 5, 1, 1);
    }
}