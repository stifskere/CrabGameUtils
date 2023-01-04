using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("heaven")]
public class Heaven : TextureReplacerTexture
{
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Color _ambientColor = default!;
    
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
    }

    public override void Update()
    {
        
    }

    public override void Enable()
    {
        Light light = Camera.main!.gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.renderMode = LightRenderMode.ForcePixel;
        light.intensity = 5;
        light.range = 30;
        _ambientColor = RenderSettings.ambientLight;
        RenderSettings.sun.color = Color.white;
        RenderSettings.ambientLight = Color.white;
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.SetColor(Color1, Color.white);
                rendererMaterial.SetColor(EmissionColor, Color.white);
            }
        }
    }

    public override void Disable()
    {
        RenderSettings.sun.color = Color.white;
        RenderSettings.ambientLight = _ambientColor;
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) continue;
            foreach (Material material in renderer.materials) Object.Destroy(material);
            renderer.materials = _materials[renderer.GetInstanceID()].ToArray();
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