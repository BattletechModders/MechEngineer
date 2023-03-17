using System.Linq;
using BattleTech;
using MechEngineer.Features.HardpointFix.Public;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitGameRep))]
public static class Mech_InitGameRep_Patch
{
    [HarmonyBefore(Mods.AC, Mods.CU)]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        var componentRefs = __instance.Weapons.Union(__instance.supportComponents)
            .Select(w => w.baseComponentRef as MechComponentRef)
            .Where(c => c != null)
            .Select(c => c!)
            .ToList();

        CalculatorSetup.Setup(__instance.MechDef.Chassis, componentRefs);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix()
    {
        CalculatorSetup.Reset();
    }
}
