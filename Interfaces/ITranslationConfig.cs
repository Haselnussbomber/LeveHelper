using LeveHelper.Enums;

namespace LeveHelper.Interfaces;

public interface ITranslationConfig
{
    public string PluginLanguage { get; set; }
    public PluginLanguageOverride PluginLanguageOverride { get; set; }
}
