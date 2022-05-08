using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.CreateMechComponentItem))]
internal static class MechLabPanel_CreateMechComponentItem_Patch
{
    [HarmonyPostfix]
    internal static void Postfix(MechLabPanel __instance, MechComponentRef componentRef, MechLabItemSlotElement __result)
    {
        try
        {
            OverrideDescriptionsFeature.Shared.AdjustSlotElement(__result, __instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}