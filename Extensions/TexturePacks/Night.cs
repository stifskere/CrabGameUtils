using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("night")]
public class Night : TextureReplacerTexture
{
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
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

    public override void Enable()
    {
        Light light = Camera.main!.gameObject.AddComponent<Light>();
        light.renderMode = LightRenderMode.ForcePixel;
        light.type = LightType.Spot;
        light.intensity = 10;
        light.range = 50;
        light.spotAngle = 90;
        RenderSettings.sun.color = Color.black;
    }

    public override void Disable()
    {
        RenderSettings.sun.color = Color.white;
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