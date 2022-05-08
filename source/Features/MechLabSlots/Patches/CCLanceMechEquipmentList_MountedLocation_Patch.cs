using System;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(
    typeof(CustomComponents.Patches.LanceMechEquipmentList_SetLoadout_Patch),
    nameof(CustomComponents.Patches.LanceMechEquipmentList_SetLoadout_Patch.MountedLocation)
)]
public static class CCLanceMechEquipmentList_MountedLocation_Patch
{
    [HarmonyPostfix]
    public static void Postfix(this MechComponentRef componentRef, ref ChassisLocations __result)
    {
        try
        {
            if (__result == ChassisLocations.CenterTorso && componentRef.Is<CustomWidget>())
            {
                __result = ChassisLocations.None;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}