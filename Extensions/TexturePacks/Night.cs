using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

struct MaterialColors
{
    public Color Color;
    public Color EmissionColor;

    public MaterialColors(Color color, Color emissionColor)
    {
        Color = color;
        EmissionColor = emissionColor;
    }
}

[TextureName("night")]
public class Night : TextureReplacerTexture
{
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<MaterialColors>> _colors = default!;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Color _ambientColor;
    private bool _lightSet;
    
    public override void Start()
    {
        _lightSet = false;
        _colors = new();
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>()) _colors[renderer.GetInstanceID()] = renderer.materials.Select(material => new MaterialColors(material.GetColor(Color1), material.GetColor(EmissionColor))).ToList();
    }

    public override void Update()
    {
        if (_lightSet) return;
        Camera? main = Camera.main;
        if (main != null)
        {
            Light light = main.gameObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.renderMode = LightRenderMode.ForcePixel;
            light.intensity = 5;
            light.range = 30;
            light.spotAngle = 90;
            _lightSet = true;
        }
    }

    public override void Enable()
    {
        Camera? main = Camera.main;
        if (main != null)
        {
            Light light = main.gameObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.renderMode = LightRenderMode.ForcePixel;
            light.intensity = 5;
            light.range = 30;
            light.spotAngle = 90;
            _lightSet = true;
        }

        _ambientColor = RenderSettings.ambientLight;
        RenderSettings.sun.color = Color.black;
        RenderSettings.ambientLight = Color.black;
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_colors.ContainsKey(renderer.GetInstanceID())) _colors[renderer.GetInstanceID()] = renderer.materials.Select(material => new MaterialColors(material.GetColor(Color1), material.GetColor(EmissionColor))).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetColor(EmissionColor, Color.black);
            }
        }
    }

    public override void Disable()
    {
        RenderSettings.sun.color = Color.white;
        RenderSettings.ambientLight = _ambientColor;
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_colors.ContainsKey(renderer.GetInstanceID())) continue;
            System.Collections.Generic.List<MaterialColors> colors = _colors[renderer.GetInstanceID()];

            for (int i = 0; i < colors.Count; i++)
            {
                renderer.materials[i].SetColor(Color1, colors[i].Color);
                renderer.materials[i].SetColor(EmissionColor, colors[i].EmissionColor);
            }
        }

        Camera? main = Camera.main;
        if (main != null)
        {
            Light light = main.GetComponent<Light>();
            if (light)
                Object.Destroy(light);
        }
    }
}