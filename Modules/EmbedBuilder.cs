
namespace CrabGameUtils.Modules;

public class CAuthor
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("url")] public string? URL { get; set; }
    [JsonPropertyName("icon_url")] public string? IconURL { get; set; }
}

public class CField
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("value")] public string? Value { get; set; }
    [JsonPropertyName("inline")] public bool InLine { get; set; }
}

public class CFooter
{
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("icon_url")] public string? IconURL { get; set; }
}

public class CThumbnail
{
    [JsonPropertyName("url")] public string? URL { get; set; }
}

public class CImage
{
    [JsonPropertyName("url")] public string? URL { get; set; }
}

internal interface IEmbed
{
    public CAuthor? Author { get; set; }
    public string? Title { get; set; }
    public string? URL { get; set; }
    public string? Description { get; set; }
    public uint Color { get; set; }
    public SystemCollections.List<CField> Fields { get; }
    public CThumbnail? Thumbnail { get; set; }
    public CImage? Image { get; set; }
    public CFooter? Footer { get; set; }
}

public class EmbedBuilder : IEmbed
{

    public CAuthor? Author { get; set; }
    public string? Title { get; set; }
    public string? URL { get; set; }
    public string? Description { get; set; }
    public uint Color { get; set; }
    public SystemCollections.List<CField> Fields { get; } = new();
    public CThumbnail? Thumbnail { get; set; }
    public CImage? Image { get; set; }
    public CFooter? Footer { get; set; }
    
    
    public EmbedBuilder SetAuthor(string name, string? url = null, string? iconUrl = null)
    {
        Author = new CAuthor { Name = name, URL = url, IconURL = iconUrl };
        return this;
    }

    public EmbedBuilder SetTitle(string title)
    {
        Title = title;
        return this;
    }

    public EmbedBuilder SetUrl(string url)
    {
        URL = url;
        return this;
    }
    
    public EmbedBuilder SetDescription(string description)
    {
        Description = description;
        return this;
    }

    public EmbedBuilder SetColor(uint color)
    {
        Color = color;
        return this;
    }

    public EmbedBuilder AddField(string name, string value, bool inline = false)
    {
        Fields.Add(new CField{Name = name, Value = value, InLine = inline});
        return this;
    }

    public EmbedBuilder SetImageURL(string url)
    {
        Image = new CImage{URL = url};
        return this;
    }

    public EmbedBuilder SetThumbnailURL(string url)
    {
        Thumbnail = new CThumbnail{URL = url};
        return this;
    }

    public EmbedBuilder SetFooter(string text, string? iconUrl = null)
    {
        Footer = new CFooter { Text = text, IconURL = iconUrl };
        return this;
    }

    public Embed Build()
    {
        return new Embed(this);
    }
}

public class Embed : IEmbed
{
    public Embed(EmbedBuilder embed)
    {
        foreach (PropertyInfo prop in embed.GetType().GetProperties())
            GetType().GetProperty(prop.Name)!.SetValue(this, prop.GetValue(embed));
        
    }
    
    [JsonPropertyName("author")] public CAuthor? Author { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("url")] public string? URL { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("color")] public uint Color { get; set; }
    [JsonPropertyName("fields")] public SystemCollections.List<CField> Fields { get; } = new();
    [JsonPropertyName("thumbnail")] public CThumbnail? Thumbnail { get; set; }
    [JsonPropertyName("image")] public CImage? Image { get; set; }
    [JsonPropertyName("footer")] public CFooter? Footer { get; set; }
}