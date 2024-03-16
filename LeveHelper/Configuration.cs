using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HaselCommon;
using HaselCommon.Interfaces;
using LeveHelper.Filters;

namespace LeveHelper;

public partial class Configuration : IConfiguration
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 1;

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

public partial class Configuration
{
    [JsonIgnore]
    public int LastSavedConfigHash { get; set; }

    public void Save()
        => ConfigurationManager.Save(this);

    public string Serialize()
        => JsonSerializer.Serialize(this, ConfigurationManager.DefaultSerializerOptions);

    public static Configuration Load()
        => ConfigurationManager.Load(CURRENT_CONFIG_VERSION, Deserialize);

    public static Configuration? Deserialize(ref JsonObject config)
        => config.Deserialize<Configuration>(ConfigurationManager.DefaultSerializerOptions);
}
