using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Network;
using LeveHelper.Caches;
using LeveHelper.Tables;
using Microsoft.Extensions.Hosting;

namespace LeveHelper.Services;

[RegisterSingleton<IHostedService>(Duplicate = DuplicateStrategy.Append), AutoConstruct]
public unsafe partial class DataUpdater : IHostedService
{
    private readonly IGameInteropProvider _gameInteropProvider;
    private readonly AssignmentIssuerCache _assignmentIssuerCache;
    private readonly LeveIssuerCache _leveIssuerCache;
    private readonly LeveListTable _leveListTable;

    private Hook<PacketDispatcher.Delegates.HandleEventYieldPacket>? _hook;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hook = _gameInteropProvider.EnabledHookFromAddress<PacketDispatcher.Delegates.HandleEventYieldPacket>(
            PacketDispatcher.MemberFunctionPointers.HandleEventYieldPacket,
            HandleEventYieldPacketDetour);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        DisposeAndNull(ref _hook);
        return Task.CompletedTask;
    }

    private void HandleEventYieldPacketDetour(EventId eventId, short scene, byte yieldId, int* intParams, byte intParamCount)
    {
        _hook!.OriginalDisposeSafe(eventId, scene, yieldId, intParams, intParamCount);

        if (eventId.ContentId != EventHandlerContent.GuildLeveAssignment || scene != 0 || yieldId != 0 || intParams == null || intParamCount == 0)
            return;

        if (!_assignmentIssuerCache.TryGetValue(eventId.Id, out var eNpcBase) || !eNpcBase.IsValid)
            return;

        if (!Data.Issuers.TryGetValue(eNpcBase.RowId, out var leveQuestIds))
            return;

        var count = (ushort)intParams[0] >> 7;

        var set = new HashSet<uint>(leveQuestIds);

        for (var i = 0; i < count; i++)
        {
            var leveId = (ushort)(intParams[(i >> 1) + 2] >> (16 * ((i - 1) & 1)));
            set.Add(leveId);
        }

        Data.Issuers[eNpcBase.RowId] = set.ToArray();

        foreach (var leveId in set)
            _leveIssuerCache.Remove(leveId);

        _leveListTable.IsFilterDirty = true;
    }
}
