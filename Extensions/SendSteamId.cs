using CrabGameUtils.Modules;

namespace CrabGameUtils.Extensions;

public class SendSteamId : Extension
{
    public override void Start()
    {
        ChatBox.Instance.ForceMessage("Hello World!");
    }

    public override void Update()
    {
        
    }
}