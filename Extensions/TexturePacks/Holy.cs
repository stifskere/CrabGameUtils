using Color = UnityEngine.Color;

namespace CrabGameUtils.Extensions.TexturePacks;

[TextureName("Holy")]
public class Holy : TextureReplacerTexture
{
    private readonly System.Collections.Generic.Dictionary<int, Color> _playerColors;
    private readonly System.Collections.Generic.Dictionary<int, Texture> _playerTextures;

    public Holy()
    {
        _playerColors = new();
        _playerTextures = new();
    }
    
    public override void Start()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Enable()
    {
        foreach (KeyValuePair<ulong, CPlayer> activePlayer in GameManager.Instance.activePlayers)
        {
            foreach (Renderer renderer in activePlayer.value.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    _playerTextures.Add(material.GetInstanceID(), material.mainTexture!);
                    _playerColors.Add(material.GetInstanceID(), material.color);
                    
                    material.color = Color.white;
                    material.mainTexture = null;
                }
            }
        }
    }

    public override void Disable()
    {
        foreach (KeyValuePair<ulong, CPlayer> activePlayer in GameManager.Instance.activePlayers)
        {
            foreach (Renderer renderer in activePlayer.value.gameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    material.color = _playerColors[material.GetInstanceID()];
                    material.mainTexture = _playerTextures[material.GetInstanceID()];
                }
            }
        }
        
        _playerColors.Clear();
        _playerTextures.Clear();
    }
}