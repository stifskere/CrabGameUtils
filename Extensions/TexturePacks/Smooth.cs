using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("smooth")]
public class Smooth : TextureReplacerTexture
{
    private Texture _texture = default!;
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Material>> _materials = default!;
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

    public override void Start()
    {
        _texture = new Texture2D(512, 512);
        if (_materials != default!)
        {
            foreach (Material material in _materials.SelectMany(keyValuePair => keyValuePair.Value))
            {
                Object.Destroy(material);
            }
        }
        _materials = new();

        Events.SpawnPlayerEvent += id =>
        {
            if (!Enabled) return;
            foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
            {
                if (!_materials.ContainsKey(renderer.GetInstanceID()))
                    _materials[renderer.GetInstanceID()] =
                        renderer.materials.Select(material => new Material(material)).ToList();
                foreach (Material rendererMaterial in renderer.materials)
                {
                    int[] textures = rendererMaterial.GetTexturePropertyNameIDs();
                    foreach (int texture in textures)
                    {
                        rendererMaterial.SetTexture(texture, null);
                    }
                }
            }
        };
    }

    public override void Update()
    {
        
    }

    public override void Enable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) _materials[renderer.GetInstanceID()] = renderer.materials.Select(material => new Material(material)).ToList();
            foreach (Material rendererMaterial in renderer.materials)
            {
                int[] textures = rendererMaterial.GetTexturePropertyNameIDs();
                foreach (int texture in textures)
                {
                    rendererMaterial.SetTexture(texture, null);
                }
            }
        }
    }

    public override void Disable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            if (!_materials.ContainsKey(renderer.GetInstanceID())) continue;
            foreach (Material material in renderer.materials) Object.Destroy(material);
            renderer.materials = _materials[renderer.GetInstanceID()].ToArray();
        }
    }
}