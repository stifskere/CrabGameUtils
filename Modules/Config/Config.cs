using System.IO;

namespace CrabGameUtils.Modules.Config;

public class Config
{
    public GlobalTemplate GlobalTemplate;

    private string _path;
    
    public Config(string path)
    {
        _path = path;
        if (!File.Exists(path) || string.IsNullOrEmpty(File.ReadAllText(path)))
        {
            GlobalTemplate = new() {
                TextureReplacer = new()
                {
                    DefaultName = null
                }
            };
            
            File.Create(path).Close();
            Write();

            return;
        }
        GlobalTemplate = JsonSerializer.Deserialize<GlobalTemplate>(File.ReadAllText(path))!;
    }

    public void Write()
    {
        File.WriteAllText(_path, JsonSerializer.Serialize(GlobalTemplate, new JsonSerializerOptions{WriteIndented = true}), Encoding.UTF8);
    }
}