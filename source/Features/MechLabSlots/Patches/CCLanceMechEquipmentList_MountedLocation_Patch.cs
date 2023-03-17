using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(
    typeof(CustomComponents.Patches.LanceMechEquipmentList_SetLoadout_Patch),
    nameof(CustomComponents.Patches.LanceMechEquipmentList_SetLoadout_Patch.MountedLocation)
)]
public static class CCLanceMechEquipmentList_MountedLocation_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(this MechComponentRef componentRef, ref ChassisLocations __result)
    {
        if (__result == ChassisLocations.CenterTorso && componentRef.Is<CustomWidget>())
        {
            __result = ChassisLocations.None;
        }
    }
}
