namespace CrabGameUtils.Modules.Config;

public class GlobalTemplate
{
    [JsonPropertyName("texture_replacer")]
    public TextureReplacerTemplate? TextureReplacer { get; set; }
}

public class TextureReplacerTemplate
{
    [JsonPropertyName("default_name")]
    public string? DefaultName { get; set; }
}