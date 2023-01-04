using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("Varying")]
public class Varying : TextureReplacerTexture
{
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
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
        
    }

    public override void Enable()
    {
        RenderSettings.sun.color = Color.white;
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                rendererMaterial.color = UnityEngine.Random.ColorHSV(0f,1f,0f,1f,0.3f,1f);
                rendererMaterial.SetColor(EmissionColor, UnityEngine.Random.ColorHSV(0f,1f,0f,1f,0.3f,1f));
            }
        }
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
        
    }
}