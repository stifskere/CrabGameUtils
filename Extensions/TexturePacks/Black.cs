using UnityEngine.Rendering;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("black")]
public class Black : TextureReplacerTexture
{
    private System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Collections.Generic.Dictionary<int, Color>>> _shaderColors;

    public Black()
    {
        _shaderColors = new();
    }
    
    public override void Start()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            int id = renderer.GetInstanceID();
            if (!_shaderColors.ContainsKey(id)) _shaderColors[renderer.GetInstanceID()] = new();
            for (int i = 0; i < renderer.materials.Count; i++)
            {
                if (_shaderColors[id].Count <= i) _shaderColors[id].Add(new());
                Shader shader = renderer.materials[i].shader;
                for (int n = 0; n < shader.GetPropertyCount(); n++)
                {
                    if (shader.GetPropertyType(n) != ShaderPropertyType.Color) continue;
                    if (!_shaderColors[id][i].ContainsKey(shader.GetPropertyNameId(n)))
                        _shaderColors[id][i][shader.GetPropertyNameId(n)] =
                            renderer.materials[i].GetColor(shader.GetPropertyNameId(n));
                }
            }
        }
    }

    public override void Update()
    {
        
    }

    public override void Enable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            int id = renderer.GetInstanceID();
            if (!_shaderColors.ContainsKey(id)) _shaderColors[renderer.GetInstanceID()] = new();
            for (int i = 0; i < renderer.materials.Count; i++)
            {
                if (_shaderColors[id].Count <= i) _shaderColors[id].Add(new());
                Shader shader = renderer.materials[i].shader;
                for (int n = 0; n < shader.GetPropertyCount(); n++)
                {
                    if (shader.GetPropertyType(n) != ShaderPropertyType.Color) continue;
                    if (!_shaderColors[id][i].ContainsKey(shader.GetPropertyNameId(n)))
                        _shaderColors[id][i][shader.GetPropertyNameId(n)] =
                            renderer.materials[i].GetColor(shader.GetPropertyNameId(n));
                }
            }
        }
        
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            int id = renderer.GetInstanceID();
            if (!_shaderColors.ContainsKey(id)) _shaderColors[renderer.GetInstanceID()] = new();
            
            for (int i = 0; i < renderer.materials.Count; i++)
            {
                if (_shaderColors[id].Count <= i) _shaderColors[id].Add(new());
                Shader shader = renderer.materials[i].shader;
                for (int n = 0; n < shader.GetPropertyCount(); n++)
                {
                    if (shader.GetPropertyType(n) != ShaderPropertyType.Color) continue;
                    if (!_shaderColors[id][i].ContainsKey(shader.GetPropertyNameId(n)))
                        _shaderColors[id][i][shader.GetPropertyNameId(n)] =
                            renderer.materials[i].GetColor(shader.GetPropertyNameId(n));
                    renderer.materials[i].SetColor(shader.GetPropertyNameId(n), Color.black);
                }
            }
        }
    }

    public override void Disable()
    {
        foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
        {
            int id = renderer.GetInstanceID();
            if (!_shaderColors.ContainsKey(id)) continue;
            
            for (int i = 0; i < renderer.materials.Count; i++)
            {
                Shader shader = renderer.materials[i].shader;
                for (int n = 0; n < shader.GetPropertyCount(); n++)
                {
                    if (shader.GetPropertyType(n) != ShaderPropertyType.Color) continue;
                    if (!_shaderColors[id][i].ContainsKey(shader.GetPropertyNameId(n))) continue;
                    renderer.materials[i].SetColor(shader.GetPropertyNameId(n), _shaderColors[id][i][shader.GetPropertyNameId(n)]);
                }
            }
        }
    }
}