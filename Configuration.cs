using System.IO;
using Dalamud.Configuration;
using LeveHelper.Filters;
using Newtonsoft.Json.Linq;

namespace LeveHelper;

[Serializable]
internal partial class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public FilterConfigs Filters { get; init; } = new();
    public bool NotifyWantedTarget { get; set; } = true;
    public bool NotifyTreasure { get; set; } = true;
}

internal class FilterConfigs
{
    public TypeFilterConfiguration TypeFilter { get; init; } = new();
    public LevemeteFilterConfiguration LevemeteFilter { get; init; } = new();
    public LocationFilterConfiguration LocationFilter { get; init; } = new();
    public NameFilterConfiguration NameFilter { get; init; } = new();
    public StatusFilterConfiguration StatusFilter { get; init; } = new();
}

internal partial class Configuration
{
    internal static Configuration Load()
    {
        var configPath = Service.PluginInterface.ConfigFile.FullName;

        var jsonData = File.Exists(configPath) ? File.ReadAllText(configPath) : null;
        if (string.IsNullOrEmpty(jsonData))
            return new();

        var parsed = JObject.Parse(jsonData);
        if (parsed == null)
            return new();

        // migrations here

        return parsed.ToObject<Configuration>() ?? new();
    }

    internal void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }
}
