using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Utility;
using HaselCommon.Extensions;
using LeveHelper.Filters;

namespace LeveHelper;

public partial class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public FilterConfigs Filters { get; init; } = new();
    public bool NotifyWantedTarget { get; set; } = true;
    public bool NotifyTreasure { get; set; } = true;
}

public class FilterConfigs
{
    public TypeFilterConfiguration TypeFilter { get; init; } = new();
    public LevemeteFilterConfiguration LevemeteFilter { get; init; } = new();
    public LocationFilterConfiguration LocationFilter { get; init; } = new();
    public NameFilterConfiguration NameFilter { get; init; } = new();
    public StatusFilterConfiguration StatusFilter { get; init; } = new();
}

// I really wish I could move this to HaselCommon, but I haven't found a way yet.
public partial class Configuration : IDisposable
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true
    };

    [JsonIgnore]
    public string LastSavedConfigHash = string.Empty;

    public static Configuration Load()
    {
        try
        {
            var configPath = Service.PluginInterface.ConfigFile.FullName;
            if (!File.Exists(configPath))
                return new();

            var jsonData = File.ReadAllText(configPath);
            if (string.IsNullOrEmpty(jsonData))
                return new();

            var config = JsonNode.Parse(jsonData);
            if (config is not JsonObject configObject)
                return new();

            var version = (int?)configObject[nameof(Version)] ?? 0;
            if (version == 0)
                return new();

            Migrate(version, configObject);

            var deserializedConfig = configObject.Deserialize<Configuration>(DefaultJsonSerializerOptions);
            if (deserializedConfig == null)
                return new();

            deserializedConfig.Save();

            return deserializedConfig;
        }
        catch (Exception ex)
        {
            Service.PluginLog.Error(ex, "Could not load the configuration file. Creating a new one.");

            if (!Service.TranslationManager.TryGetTranslation("Plugin.DisplayName", out var pluginName))
                pluginName = Service.PluginInterface.InternalName;

            Service.PluginInterface.UiBuilder.AddNotification(
                t("Notification.CouldNotLoadConfig"),
                pluginName,
                NotificationType.Error,
                5000
            );

            return new();
        }
    }

    public static void Migrate(int version, JsonObject config)
    {
        // Service.PluginLog.Debug("Migrate called: {version} - {config}", version, config);
    }

    public void Save()
    {
        try
        {
            var serialized = JsonSerializer.Serialize(this, DefaultJsonSerializerOptions);
            var hash = serialized.GetHash();

            if (LastSavedConfigHash != hash)
            {
                Util.WriteAllTextSafe(Service.PluginInterface.ConfigFile.FullName, serialized);
                LastSavedConfigHash = hash;
                Service.PluginLog.Information("Configuration saved.");
            }
        }
        catch (Exception e)
        {
            Service.PluginLog.Error(e, "Error saving config");
        }
    }

    void IDisposable.Dispose()
    {
        Save();
    }
}
