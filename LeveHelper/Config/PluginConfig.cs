using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using LeveHelper.Filters;

namespace LeveHelper.Config;

public partial class PluginConfig : IPluginConfiguration
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 1;

    [JsonIgnore]
    public int LastSavedConfigHash { get; set; }

    [JsonIgnore]
    public static JsonSerializerOptions? SerializerOptions { get; private set; } = new()
    {
        IncludeFields = true,
        WriteIndented = true,
    };

    [JsonIgnore]
    private static IDalamudPluginInterface? PluginInterface;

    [JsonIgnore]
    private static IPluginLog? PluginLog;

    public static PluginConfig Load(
        IDalamudPluginInterface pluginInterface,
        IPluginLog pluginLog)
    {
        PluginInterface = pluginInterface;
        PluginLog = pluginLog;

        var fileInfo = pluginInterface.ConfigFile;
        if (!fileInfo.Exists || fileInfo.Length < 2)
            return new();

        var json = File.ReadAllText(fileInfo.FullName);
        var node = JsonNode.Parse(json);
        if (node == null)
            return new();

        if (node is not JsonObject config)
            return new();

        var version = config[nameof(Version)]?.GetValue<int>();
        if (version == null)
            return new();

        return JsonSerializer.Deserialize<PluginConfig>(node, SerializerOptions) ?? new();
    }

    public void Save()
    {
        try
        {
            var serialized = JsonSerializer.Serialize(this, SerializerOptions);
            var hash = serialized.GetHashCode();

            if (LastSavedConfigHash != hash)
            {
                Util.WriteAllTextSafe(PluginInterface!.ConfigFile.FullName, serialized);
                LastSavedConfigHash = hash;
                PluginLog?.Information("Configuration saved.");
            }
        }
        catch (Exception e)
        {
            PluginLog?.Error(e, "Error saving config");
        }
    }
}

public partial class PluginConfig
{
    public int Version { get; set; } = CURRENT_CONFIG_VERSION;
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
