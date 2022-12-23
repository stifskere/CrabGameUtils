namespace CrabGameUtils.Modules;

public class ButtonBuilder
{
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("url")] public string? URL { get; set; }

    public ButtonBuilder SetLabel(string label)
    {
        Label = label;
        return this;
    }

    public ButtonBuilder SetURL(string url)
    {
        URL = url;
        return this;
    }

    public Button Build()
    {
        return new Button(this);
    }
}

public class Button
{
    public Button(ButtonBuilder button)
    {
        Label = button.Label;
        URL = button.URL;
    }
    
    [JsonPropertyName("type")] public byte Type { get; set; } = 2;
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("style")] public byte Style { get; set; } = 5;
    [JsonPropertyName("url")] public string? URL { get; set; }
}

public class Components
{
    [JsonPropertyName("type")] public byte Type { get; set; } = 1;
    [JsonPropertyName("components")] public Button[]? ComponentsButtons { get; set; }
}