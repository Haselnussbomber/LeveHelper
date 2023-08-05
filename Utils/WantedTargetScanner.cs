using System.Collections.Generic;
using Dalamud;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using LeveHelper.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace LeveHelper;

public static class WantedTargetScanner
{
    private static bool IsSubscribed;

    // name ids (= rowid of BNpcName sheet)
    private static readonly List<uint> WantedTargetIds = new()
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

    private static DateTime LastCheck = DateTime.Now;
    private static readonly List<uint> FoundWantedTargets = new();
    private static readonly List<uint> FoundTreasures = new();

    public static unsafe void Connect()
    {
        if (IsSubscribed)
            return;

        DirectorHelper.DirectorChanged += OnDirectorChanged;
        Service.Framework.Update += Framework_Update;

        IsSubscribed = true;
    }

    public static unsafe void Disconnect()
    {
        DirectorHelper.DirectorChanged -= OnDirectorChanged;
        Service.Framework.Update -= Framework_Update;

        IsSubscribed = false;
    }

    private static unsafe void OnDirectorChanged(Director* NewDirector)
    {
        FoundWantedTargets.Clear();
        FoundTreasures.Clear();
    }

    private static void Framework_Update(Dalamud.Game.Framework framework)
    {
        var config = Plugin.Config;

        if (!config.NotifyTreasure && !config.NotifyWantedTarget)
        {
            return;
        }

        if (DateTime.Now - LastCheck < TimeSpan.FromSeconds(1))
        {
            return;
        }

        LastCheck = DateTime.Now;

        DirectorHelper.Update();

        if (!DirectorHelper.IsBattleLeveDirectorActive)
        {
            return;
        }

        var territoryTypeId = Service.ClientState.TerritoryType;
        if (territoryTypeId == 0) return;

        var territoryType = GetRow<TerritoryType>(territoryTypeId);
        if (territoryType == null) return;

        foreach (var obj in Service.ObjectTable)
        {
            if (config.NotifyTreasure
                && obj.ObjectKind == ObjectKind.Treasure
                && !FoundTreasures.Contains(obj.ObjectId))
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

                FoundTreasures.Add(obj.ObjectId);
                continue;
            }

            if (config.NotifyWantedTarget
                && obj.ObjectKind == ObjectKind.BattleNpc
                && obj.SubKind == (byte)BattleNpcSubKind.Enemy
                && !FoundWantedTargets.Contains(obj.ObjectId)
                && obj is BattleNpc battleNpc
                && WantedTargetIds.Contains(battleNpc.NameId))
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

                FoundWantedTargets.Add(obj.ObjectId);
            }
        }
    }
}
