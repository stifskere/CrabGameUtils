namespace CrabGameUtils.Modules;

public class Config
{
    [JsonPropertyName("defaultname")]
    public string? DefaultName { get; set; }

    public Config()
    {
        DefaultName = null;
    }

    public static Config Load(string filePath)
    {
        if (!File.Exists(filePath)) File.Create(filePath).Close();
        string jsonString = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(jsonString))
        {
            return new Config();
        }

        Config config = JsonSerializer.Deserialize<Config>(jsonString)!;
        config.DefaultName ??= null;
        return config;
    }

    public void Save()
    {
        string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions{WriteIndented = true});
        File.WriteAllText(_filePath!, jsonString);
    }

    public Config(string filePath)
    {
        _filePath = filePath;
    }
    
    private string? _filePath;
}