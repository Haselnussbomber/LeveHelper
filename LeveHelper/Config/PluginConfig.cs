using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using LeveHelper.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace LeveHelper.Config;

public partial class PluginConfig : IPluginConfiguration
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 2;

    [JsonIgnore]
    public int LastSavedConfigHash { get; set; }

    [JsonIgnore]
    public static JsonSerializerOptions? SerializerOptions { get; } = new()
    {
        IncludeFields = true,
        WriteIndented = true,
    };

    [JsonIgnore]
    private static IDalamudPluginInterface? PluginInterface;

    [JsonIgnore]
    private static IPluginLog? PluginLog;

    public static PluginConfig Load(IDalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
        PluginLog = pluginInterface.GetRequiredService<IPluginLog>();

        var fileInfo = PluginInterface.ConfigFile;
        if (!fileInfo.Exists || fileInfo.Length < 2)
            return new();

        var json = File.ReadAllText(fileInfo.FullName);
        var node = JsonNode.Parse(json);
        if (node is not JsonObject)
            return new();

        return JsonSerializer.Deserialize<PluginConfig>(node, SerializerOptions) ?? new();
    }

    public void Save()
    {
        try
        {
            var serialized = JsonSerializer.Serialize(this, SerializerOptions);
            var hash = StringComparer.Ordinal.GetHashCode(serialized);

            if (LastSavedConfigHash != hash)
            {
                FilesystemUtil.WriteAllTextSafe(PluginInterface!.ConfigFile.FullName, serialized);
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
    public FilterConfig Filters { get; init; } = new();
    public bool NotifyWantedTarget { get; set; } = true;
    public bool NotifyTreasure { get; set; } = true;
    public bool ShowImportOnTeamCraftButton { get; set; } = false;
}

public class FilterConfig
{
    // RowIdColumn
    public uint RowId = 0;

    // LevelColumn
    public int MinLevel = 0;
    public int MaxLevel = 0;

    // LevemeteColumn
    public int Levemete = 0;

    // NameColumn
    public string Name = string.Empty;

    // StatusColumn
    public LeveStatus Status = LeveStatus.Any;

    // TypeColumn
    public uint Type = 0;
}

public static class PluginConfigExtension
{
    public static void AddConfig(this IServiceCollection services, PluginConfig pluginConfig)
    {
        services.AddSingleton(pluginConfig);
    }
}
