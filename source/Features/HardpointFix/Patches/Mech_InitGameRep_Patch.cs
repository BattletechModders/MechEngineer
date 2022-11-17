using System;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.Public;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitGameRep))]
public static class Mech_InitGameRep_Patch
{
    [HarmonyBefore(Mods.AC, Mods.CU)]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static void Prefix(Mech __instance)
    {
        try
        {
            var componentRefs = __instance.Weapons.Union(__instance.supportComponents)
                .Select(w => w.baseComponentRef as MechComponentRef)
                .Where(c => c != null)
                .Select(c => c!)
                .ToList();

            CalculatorSetup.Setup(__instance.MechDef.Chassis, componentRefs);
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
