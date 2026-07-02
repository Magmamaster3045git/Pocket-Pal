using System.IO;
using System.Text.Json;

namespace PocketPal.Settings;

/// <summary>
/// Handles reading/writing AppSettings to disk. Isolated here so a future
/// "save game state" system can follow the exact same pattern for pet
/// stats/inventory without redesigning persistence from scratch.
/// </summary>
public sealed class SettingsManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public SettingsManager()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PocketPal");

        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "settings.json");
    }

    public AppSettings Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (settings is not null) return settings;
            }
        }
        catch (Exception)
        {
            // Corrupt or unreadable settings file - fall through to defaults
            // rather than crashing the app on startup.
        }

        return new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception)
        {
            // Best-effort save; a failed write shouldn't crash the app.
        }
    }
}
