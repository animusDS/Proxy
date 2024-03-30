using System.Text.Json;

namespace Proxy;

public class Settings {
    private static readonly Logger Log = new(typeof(Settings));

    private Dictionary<string, PluginSettings> _pluginSettings = new();

    public static Settings Load() {
        var filePath = Path.Combine(Environment.CurrentDirectory, "settings.json");

        try {
            if (File.Exists(filePath)) {
                var json = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, PluginSettings>>(json);
                if (settings != null)
                    return new Settings {
                        _pluginSettings = settings,
                    };


                Log.Info("Error loading settings");
                return new Settings();
            }
        }
        catch (Exception ex) {
            Log.Error($"Error loading settings: {ex.Message}");
        }

        Log.Info("Settings not found, creating new settings file");

        return new Settings();
    }

    public void Save() {
        var filePath = Path.Combine(Environment.CurrentDirectory, "settings.json");

        try {
            var json = JsonSerializer.Serialize(_pluginSettings, new JsonSerializerOptions {
                WriteIndented = true,
            });

            File.WriteAllText(filePath, json);
        }
        catch (Exception ex) {
            Log.Info($"Error saving settings: {ex.Message}");
        }
    }

    public PluginSettings GetPluginSettings(string pluginIdentifier) {
        if (_pluginSettings.TryGetValue(pluginIdentifier, out PluginSettings settings))
            return settings;

        var newSettings = new PluginSettings();
        _pluginSettings.Add(pluginIdentifier, newSettings);

        return newSettings;
    }

    public void SetPluginSettings(string pluginIdentifier, PluginSettings settings, bool save = false) {
        _pluginSettings[pluginIdentifier] = settings;

        if (save)
            Save();
    }
}

public class PluginSettings {
    public bool Enabled;

    public Dictionary<string, object> OtherOptions = new();

    public void SetOption(string optionName, object optionValue) {
        OtherOptions[optionName] = optionValue;
    }

    public T GetOption<T>(string optionName) {
        if (OtherOptions.TryGetValue(optionName, out object value) && value is T optionValue)
            return optionValue;

        return default;
    }
}