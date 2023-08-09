using System.Collections.Generic;
using Dalamud;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using LeveHelper.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper.Services;

public unsafe class WantedTargetScanner : IDisposable
{
    // name ids (= rowid of BNpcName sheet)
    private readonly List<uint> _wantedTargetIds = new()
    {
        471, // Angry Sow
        472, // Rotting Sentinel
        473, // Mischief-maker Imp
        474, // Seven-year Gnat
        475, // Alpha Stag
        476, // Wanted Goblin
        477, // Lush Morbol
        1008, // Fernehalwes
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
        7169, // Sabotender Corrido
    };

    private readonly List<uint> _foundWantedTargets = new();
    private readonly List<uint> _foundTreasures = new();
    private DateTime _lastCheck = DateTime.Now;
    private Director* _lastDirector;

    public WantedTargetScanner()
    {
        Service.Framework.Update += Framework_Update;
    }

    public void Dispose()
    {
        Service.Framework.Update -= Framework_Update;
    }

    public bool IsBattleLeveDirector(Director* director)
        => director != null &&
           director->EventHandlerInfo != null &&
           director->EventHandlerInfo->EventId.Type == EventHandlerType.BattleLeveDirector;

    private void Framework_Update(Dalamud.Game.Framework framework)
    {
        var config = Plugin.Config;

        if (!config.NotifyTreasure && !config.NotifyWantedTarget)
            return;

        if (DateTime.Now - _lastCheck < TimeSpan.FromSeconds(1))
            return;

        _lastCheck = DateTime.Now;

        var activeDirector = UIState.Instance()->ActiveDirector;
        if (_lastDirector != activeDirector)
        {
            _lastDirector = activeDirector;
            _foundWantedTargets.Clear();
            _foundTreasures.Clear();
        }

        if (!IsBattleLeveDirector(activeDirector))
            return;

        var territoryTypeId = Service.ClientState.TerritoryType;
        if (territoryTypeId == 0) return;

        var territoryType = GetRow<TerritoryType>(territoryTypeId);
        if (territoryType == null) return;

        foreach (var obj in Service.ObjectTable)
        {
            if (config.NotifyTreasure
                && obj.ObjectKind == ObjectKind.Treasure
                && !_foundTreasures.Contains(obj.ObjectId))
            {
                var mapLink = obj.GetMapLink();
                if (mapLink == null) continue;

                var sb = new SeStringBuilder();

                sb.AddUiForeground(69);
                sb.AddText("[LeveHelper] ");
                sb.AddUiForegroundOff();

                sb.Append(mapLink);

                sb.AddText(Service.ClientState.ClientLanguage switch
                {
                    ClientLanguage.Japanese => "発見されました。",
                    ClientLanguage.German => " entdeckt.",
                    ClientLanguage.French => " découvert.",
                    _ => " discovered."
                });

                Service.ChatGui.Print(sb.Build());

                _foundTreasures.Add(obj.ObjectId);
                continue;
            }

            if (config.NotifyWantedTarget
                && obj.ObjectKind == ObjectKind.BattleNpc
                && obj.SubKind == (byte)BattleNpcSubKind.Enemy
                && !_foundWantedTargets.Contains(obj.ObjectId)
                && obj is BattleNpc battleNpc
                && _wantedTargetIds.Contains(battleNpc.NameId))
            {
                var mapLink = obj.GetMapLink();
                if (mapLink == null) continue;

                var sb = new SeStringBuilder();

                sb.AddUiForeground(69);
                sb.AddText("[LeveHelper] ");
                sb.AddUiForegroundOff();
                sb.AddText(Service.ClientState.ClientLanguage switch
                {
                    ClientLanguage.Japanese => "捜査対象 ",
                    ClientLanguage.German => "Gesuchtes Ziel ",
                    ClientLanguage.French => "Cible recherchée ",
                    _ => "Wanted target "
                });

                sb.Append(mapLink);

                sb.AddText(Service.ClientState.ClientLanguage switch
                {
                    ClientLanguage.Japanese => "発見されました。",
                    ClientLanguage.German => " entdeckt.",
                    ClientLanguage.French => " découvert.",
                    _ => " discovered."
                });

                Service.ChatGui.Print(sb.Build());

                _foundWantedTargets.Add(obj.ObjectId);
            }
        }
    }
}
