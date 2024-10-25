using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Game;
using HaselCommon.Services;
using LeveHelper.Config;
using Lumina.Text;

namespace LeveHelper.Services;

public unsafe class WantedTargetScanner : IDisposable
{
    private readonly IFramework Framework;
    private readonly IObjectTable ObjectTable;
    private readonly PluginConfig PluginConfig;
    private readonly TextService TextService;
    private readonly MapService MapService;

    // name ids (= rowid of BNpcName sheet)
    private readonly List<uint> WantedTargetIds =
    [
        471, // Angry Sow
        472, // Rotting Sentinel
        473, // Mischief-maker Imp
        474, // Seven-year Gnat
        475, // Alpha Stag
        476, // Wanted Goblin
        477, // Lush Morbol
        1008, // Fernehalwes
        1009, // Sabotender Corrido
        1010, // Masterless Thrall
        1011, // Scale Eater
        1012, // Pinktoe
        1013, // Greataxe Beak
        1052, // Mamool Ja Menace
        1053, // Longarm
        1054, // Thunderhooves
        1055, // Rageclaw
        1056, // Warren Warden
        1057, // Ripe Rampager
        1058, // Cepheus
        1764, // Skadi
        1765, // Great White Torama
        1776, // Gorn the Garrgh
        3716, // Zaghnal
        3717, // Mighty Mackerel
        3718, // The Scarlet Lector
        3719, // Xexeu
        3720, // Tcaridyi
    ];

    private readonly List<uint> FoundWantedTargets = [];
    private readonly List<uint> FoundTreasures = [];
    private DateTime LastCheck = DateTime.Now;
    private Director* LastDirector;

    public WantedTargetScanner(
        IFramework framework,
        IObjectTable objectTable,
        PluginConfig pluginConfig,
        TextService textService,
        MapService mapService)
    {
        Framework = framework;
        ObjectTable = objectTable;
        PluginConfig = pluginConfig;
        TextService = textService;
        MapService = mapService;

        Framework.Update += Framework_Update;
    }

    public void Dispose()
    {
        Framework.Update -= Framework_Update;
        GC.SuppressFinalize(this);
    }

    public bool IsBattleLeveDirector(Director* director)
        => director != null &&
           director->EventHandlerInfo != null &&
           director->EventHandlerInfo->EventId.ContentId == EventHandlerType.BattleLeveDirector;

    private void Framework_Update(IFramework framework)
    {
        if (!PluginConfig.NotifyTreasure && !PluginConfig.NotifyWantedTarget)
            return;

        if (DateTime.Now - LastCheck < TimeSpan.FromSeconds(1))
            return;

        LastCheck = DateTime.Now;

        var activeDirector = UIState.Instance()->DirectorTodo.Director;
        if (LastDirector != activeDirector)
        {
            LastDirector = activeDirector;
            FoundWantedTargets.Clear();
            FoundTreasures.Clear();
        }

        if (!IsBattleLeveDirector(activeDirector))
            return;

        foreach (var obj in ObjectTable)
        {
            if (PluginConfig.NotifyTreasure
                && obj.ObjectKind == ObjectKind.Treasure
                && !FoundTreasures.Contains(obj.EntityId))
            {
                var mapLink = MapService.GetMapLink(obj);
                if (mapLink == null)
                    continue;

                Chat.Print(new SeStringBuilder()
                    .PushColorType(69)
                    .Append("[LeveHelper] ")
                    .PopColorType()
                    .Append(TextService.TranslateSeString("WantedTargetScanner.Treasure.Notification", mapLink.Value))
                    .ToReadOnlySeString());

                FoundTreasures.Add(obj.EntityId);
            }

            if (PluginConfig.NotifyWantedTarget
                && obj.ObjectKind == ObjectKind.BattleNpc
                && obj.SubKind == (byte)BattleNpcSubKind.Enemy
                && !FoundWantedTargets.Contains(obj.EntityId)
                && obj is IBattleNpc battleNpc
                && WantedTargetIds.Contains(battleNpc.NameId))
            {
                var mapLink = MapService.GetMapLink(obj);
                if (mapLink == null)
                    continue;

                Chat.Print(new SeStringBuilder()
                    .PushColorType(69)
                    .Append("[LeveHelper] ")
                    .PopColorType()
                    .Append(TextService.TranslateSeString("WantedTargetScanner.WantedTarget.Notification", mapLink.Value))
                    .ToReadOnlySeString());

                FoundWantedTargets.Add(obj.EntityId);
            }
        }
    }
}
