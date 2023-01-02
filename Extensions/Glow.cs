
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions;

[ExtensionName("Player glow")]
public class Glow : Extension
{
    public ExtensionConfig<string> Key = new("key", "i", "the key to enable or disable ");
    
    private bool _enabled = true;
    private GameObject _sun = null!;
    private Color _defaultAmbient;
    private System.Collections.Generic.Dictionary<int, Color> _defaultObjectColors = new();

    public override void Start()
    {
         _defaultObjectColors = new();
        if (!Enabled.Value) return;
        
        if (!System.Enum.TryParse(Key.Value.ToUpper(), out KeyCode _))
        {
            ThrowError("Party errored, the keycode is not valid.");
            return;
        }

        _sun = GameObject.Find("Directional Light");
        _defaultAmbient = RenderSettings.ambientLight;
        
        foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>())
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (!renderer) continue;
            
            _defaultObjectColors.Add(gameObject.GetInstanceID(), renderer.material.color);
        }

        ChatBox.Instance.ForceMessage($"<color=#00FFFF>Party Loaded, press {Key.Value} to go on drugs.</color>");
    }

    public override void Update()
    {
        if (!Enabled.Value) return;
        
        if (Input.GetKeyDown(Key.Value.ToLower()) && !ChatBox.Instance.inputField.isFocused)
        {
            _enabled = !_enabled;
            ChatBox.Instance.ForceMessage($"<color=orange>Party mode:</color> {(_enabled ? "<color=green>enabled</color>" : "<color=red>disabled</color>")}");
        }

        _sun.active = !_enabled;
        RenderSettings.ambientLight = _enabled ? Color.black : _defaultAmbient;

        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            GameObject light = GameObject.Find($"Light-{player.Key}");
            
            if (!_enabled)
            {
                if (light) Object.Destroy(light);
                continue;
            }
            
            if (!light) light = new GameObject($"Light-{player.Key}");
            Light lightComp = light.GetComponent<Light>();

            if (!lightComp)
            {
                lightComp = light.AddComponent<Light>();
                lightComp.type = LightType.Point;
                lightComp.intensity = 5;
                lightComp.range = 30;
                lightComp.bounceIntensity = 1;
                lightComp.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
            }
            
            Color.RGBToHSV(lightComp.color, out float value, out _, out _);
            lightComp.color = value + Time.deltaTime / 5 > 1 ? Color.HSVToRGB(0, 1, 1) : Color.HSVToRGB(value + Time.deltaTime / 5, 1, 1);
            light.transform.position = player.Value.transform.position + new Vector3(0, 2, 0);
        }

        foreach (GameObject gameObject in Object.FindObjectsOfType<GameObject>())
        {
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (!renderer) continue;
            
            if (!_enabled)
            {
                int id = gameObject.GetInstanceID();
                if (!_defaultObjectColors.ContainsKey(id)) continue;
                renderer.material.color = _defaultObjectColors[id];
                continue;
            }
            
            Color.RGBToHSV(renderer.material.color, out float h, out _, out _);
            renderer.material.color = Color.HSVToRGB(h + Time.deltaTime / 5 >= 1 ? 0 : h + Time.deltaTime / 5, 1, 1);
        }
    }
}