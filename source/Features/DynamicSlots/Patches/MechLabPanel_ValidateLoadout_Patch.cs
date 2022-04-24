using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ValidateLoadout))]
public static class MechLabPanel_ValidateLoadout_Patch
{
    public static void Postfix(MechLabPanel __instance)
    {
        try
        {
            DynamicSlotsFeature.Shared.RefreshData(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}