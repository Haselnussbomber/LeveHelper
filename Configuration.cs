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

internal partial class Configuration : IDisposable
{
    private static Configuration instance = null!;
    public static Configuration Instance => instance ??= Load();

    internal static Configuration Load()
    {
        if (instance != null)
            return instance;

        var configPath = Service.PluginInterface.ConfigFile.FullName;

        var jsonData = File.Exists(configPath) ? File.ReadAllText(configPath) : null;
        if (string.IsNullOrEmpty(jsonData))
            return instance = new Configuration();

        var parsed = JObject.Parse(jsonData);
        if (parsed == null)
            return instance = new Configuration();

        // migrations here

        return instance = parsed.ToObject<Configuration>() ?? new Configuration();
    }

    internal static void Save()
    {
        Service.PluginInterface.SavePluginConfig(instance);
    }

    void IDisposable.Dispose()
    {
        instance = null!;
    }
}
