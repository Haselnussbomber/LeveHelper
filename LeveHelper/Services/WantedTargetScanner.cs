using System.Collections.Generic;
using System.Linq;
using AutoCtor;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HaselCommon.Game;
using HaselCommon.Services;
using LeveHelper.Config;
using Lumina.Text;
using EventHandler = FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler;

namespace LeveHelper.Services;

[RegisterTransient, AutoConstruct]
public unsafe partial class WantedTargetScanner : IDisposable
{
    // name ids (= rowid of BNpcName sheet)
    private static readonly uint[] WantedTargetIds =
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

    private readonly IFramework _framework;
    private readonly IObjectTable _objectTable;
    private readonly PluginConfig _pluginConfig;
    private readonly TextService _textService;
    private readonly MapService _mapService;

    private readonly List<uint> _foundWantedTargets = [];
    private readonly List<uint> _foundTreasures = [];
    private DateTime _lastCheck = DateTime.Now;
    private Director* _lastDirector;

    [AutoPostConstruct]
    private void Initialize()
    {
        _framework.Update += Framework_Update;
    }

    public void Dispose()
    {
        _framework.Update -= Framework_Update;
        GC.SuppressFinalize(this);
    }

    public bool IsBattleLeveDirector(EventHandler* eventHandler)
        => eventHandler != null &&
           eventHandler->Info.EventId.ContentId == EventHandlerContent.BattleLeveDirector;

    private void Framework_Update(IFramework framework)
    {
        if (!_pluginConfig.NotifyTreasure && !_pluginConfig.NotifyWantedTarget)
            return;

        if (DateTime.Now - _lastCheck < TimeSpan.FromSeconds(1))
            return;

        _lastCheck = DateTime.Now;

        var activeDirector = UIState.Instance()->DirectorTodo.Director;
        if (_lastDirector != activeDirector)
        {
            _lastDirector = activeDirector;
            _foundWantedTargets.Clear();
            _foundTreasures.Clear();
        }

        if (!IsBattleLeveDirector((EventHandler*)activeDirector))
            return;

        foreach (var obj in _objectTable)
        {
            if (_pluginConfig.NotifyTreasure
                && obj.ObjectKind == ObjectKind.Treasure
                && !_foundTreasures.Contains(obj.EntityId))
            {
                var mapLink = _mapService.GetMapLink(obj);
                if (mapLink == null)
                    continue;

                Chat.Print(new SeStringBuilder()
                    .PushColorType(69)
                    .Append("[LeveHelper] ")
                    .PopColorType()
                    .Append(_textService.TranslateSeString("WantedTargetScanner.Treasure.Notification", mapLink.Value))
                    .ToReadOnlySeString());

                _foundTreasures.Add(obj.EntityId);
            }

            if (_pluginConfig.NotifyWantedTarget
                && obj.ObjectKind == ObjectKind.BattleNpc
                && obj.SubKind == (byte)BattleNpcSubKind.Enemy
                && !_foundWantedTargets.Contains(obj.EntityId)
                && obj is IBattleNpc battleNpc
                && WantedTargetIds.Contains(battleNpc.NameId))
            {
                var mapLink = _mapService.GetMapLink(obj);
                if (mapLink == null)
                    continue;

                Chat.Print(new SeStringBuilder()
                    .PushColorType(69)
                    .Append("[LeveHelper] ")
                    .PopColorType()
                    .Append(_textService.TranslateSeString("WantedTargetScanner.WantedTarget.Notification", mapLink.Value))
                    .ToReadOnlySeString());

                _foundWantedTargets.Add(obj.EntityId);
            }
        }
    }
}
