using System;
using BattleTech.UI;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ValidateLoadout))]
public static class MechLabPanel_ValidateLoadout_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            DynamicSlotsFeature.Shared.RefreshData(__instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
