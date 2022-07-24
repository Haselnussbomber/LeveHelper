using System;
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
}

internal class FilterConfigs
{
    public ClassFilter.Configuration ClassFilter { get; init; } = new();
    public LevemeteFilter.Configuration LevemeteFilter { get; init; } = new();
    public LocationFilter.Configuration LocationFilter { get; init; } = new();
    public NameFilter.Configuration NameFilter { get; init; } = new();
    public StatusFilter.Configuration StatusFilter { get; init; } = new();
}

internal partial class Configuration
{
    internal static Configuration Load(Plugin plugin)
    {
        var configPath = plugin.PluginInterface.ConfigFile.FullName;

        string? jsonData = File.Exists(configPath) ? File.ReadAllText(configPath) : null;
        if (string.IsNullOrEmpty(jsonData))
            return new Configuration();

        var config = JObject.Parse(jsonData);
        if (config == null)
            return new Configuration();

        // migrations here

        var configuration = config.ToObject<Configuration>();
        if (configuration == null)
            return new Configuration();

        return configuration;
    }

    internal void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }
}
