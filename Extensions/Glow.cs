using Color = UnityEngine.Color;
using TimeSpan = System.TimeSpan;

namespace CrabGameUtils.Extensions;

[ExtensionName("Player glow")]
public class Glow : Extension
{
    private MethodRunDelay _addLightsDelay;
    
    public override void Start()
    {
        _addLightsDelay = new MethodRunDelay(TimeSpan.FromSeconds(3), DelayAdd);
    }

    public override void Update()
    {
        _addLightsDelay.WaitAndRun();
    }

    private void DelayAdd()
    {
        foreach (KeyValuePair<ulong, CPlayer> player in GameManager.Instance.activePlayers)
        {
            Random currentRand = new Random();
            Light lightComp = player.Value.gameObject.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.color = new Color(currentRand.Next(255), currentRand.Next(255), currentRand.Next(255));
            lightComp.intensity = 0.5f;
            lightComp.range = 5;
        }
    }
}