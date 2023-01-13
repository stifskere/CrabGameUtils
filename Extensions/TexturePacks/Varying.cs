using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("Varying")]
public class Varying : TextureReplacerTexture
{
    public ExtensionConfig<int> R = new("Red Value", 0 , "Defines the Red value of a color, disabled at 0(max 255 || min 0)");
    public ExtensionConfig<int> G = new("Green Color", 0 , "Defines the Green value of a color, disabled at 0(max 255 || min 0)");
    public ExtensionConfig<int> B = new("Blue Color", 0 , "Defines the Blue value of a color, disabled at 0 (max 255 || min 0)");
    public ExtensionConfig<int> Randomness  = new("Randomness", 20 , "How close other colors are to the target one, (max 100 || min 0)");

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
            
            //if disabled
            if (R.Value + G.Value + B.Value == 0)
            {
                foreach (Material rendererMaterial in renderer.materials)
                {
                    rendererMaterial.color = UnityEngine.Random.ColorHSV(0f,1f,0f,1f,0.3f,1f);
                    rendererMaterial.SetColor(EmissionColor, UnityEngine.Random.ColorHSV(0f,1f,0f,1f,0.3f,1f));
                }
            }
            else //if enabled
            {
                Color.RGBToHSV(new Color(FixValue(R.Value,0,255),FixValue(G.Value,0,255),FixValue(B.Value,0,255)), out float h, out _, out _);

                float fixedRandomness = FixValue((float)Randomness.Value / 100, 0, 1);
                float minhue = FixValue(h - fixedRandomness, 0, 1);
                float maxhue = FixValue(h + fixedRandomness, 0, 1);
                
                
                foreach (Material rendererMaterial in renderer.materials)
                {
                    if (rendererMaterial.mainTexture)
                    {
                        Texture2D dest = new Texture2D(rendererMaterial.mainTexture.width, rendererMaterial.mainTexture.height, TextureFormat.RGBA32, false);
                        Texture end = MakeGrayscale(dest);
                        rendererMaterial.mainTexture = end;
                    }
                    rendererMaterial.color = UnityEngine.Random.ColorHSV(minhue,maxhue);
                    rendererMaterial.SetColor(EmissionColor, UnityEngine.Random.ColorHSV(minhue,maxhue));
                }
                
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

    
    Texture MakeGrayscale (Texture2D tex) 
    {
        var texColors = tex.GetPixels();
        for (int i = 0; i < texColors.Length; i++) 
        {
            var grayValue = texColors[i].grayscale;
            texColors[i] = new Color(grayValue, grayValue, grayValue, texColors[i].a);
        }
        tex.SetPixels(texColors);
        tex.Apply();
        return tex;
    }
}