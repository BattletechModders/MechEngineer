using System;
using System.Linq;
using BattleTech;
using MechEngineer.Features.HardpointFix.Public;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(MechRepresentationSimGame), nameof(MechRepresentationSimGame.LoadWeapons))]
public static class MechRepresentationSimGame_LoadWeapons_Patch
{
    [HarmonyBefore(Mods.CU)]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechRepresentationSimGame __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            CalculatorSetup.Setup(
                __instance.mechDef?.Chassis,
                __instance.mechDef?.Inventory?.ToList());
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        CalculatorSetup.Reset();
    }
}
