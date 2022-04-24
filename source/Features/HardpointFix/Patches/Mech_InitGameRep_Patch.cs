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
    public static void Prefix(Mech __instance)
    {
        try
        {
            var componentRefs = __instance.Weapons.Union(__instance.supportComponents)
                .Select(w => w.baseComponentRef as MechComponentRef)
                .Where(c => c != null)
                .ToList();

            CalculatorSetup.Setup(__instance.MechDef.Chassis, componentRefs);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    public static void Postfix()
    {
        CalculatorSetup.Reset();
    }
}