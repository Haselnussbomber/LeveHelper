using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace LeveHelper.Services;

[RegisterSingleton]
public class GatherBuddyIpcProvider
{
    private readonly ICallGateSubscriber<uint, uint?, Vector2, Vector2, Vector2, bool, object> _drawFishTooltip;

    public GatherBuddyIpcProvider(IDalamudPluginInterface pluginInterface)
    {
        _drawFishTooltip = pluginInterface.GetIpcSubscriber<uint, uint?, Vector2, Vector2, Vector2, bool, object>("GatherBuddy.DrawFishToolitp");
    }

    public bool DrawTooltip(uint fishId, uint? territoryId)
    {
        try
        {
            _drawFishTooltip.InvokeAction(
                fishId,
                territoryId != 0 ? territoryId : null,
                ImGuiHelpers.ScaledVector2(40, 40),
                ImGuiHelpers.ScaledVector2(20, 20),
                ImGuiHelpers.ScaledVector2(30, 30),
                false);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
