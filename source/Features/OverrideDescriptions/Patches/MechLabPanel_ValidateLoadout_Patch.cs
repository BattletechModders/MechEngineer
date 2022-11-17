using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ValidateLoadout))]
public static class MechLabPanel_ValidateLoadout_Patch
{
    [HarmonyPostfix]
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            OverrideDescriptionsFeature.Shared.RefreshData(__instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
