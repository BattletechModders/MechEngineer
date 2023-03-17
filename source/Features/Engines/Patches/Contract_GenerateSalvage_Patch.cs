using System;
using System.Collections.Generic;
using BattleTech;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.Engines.Patches;

[HarmonyPatch(typeof(Contract), nameof(Contract.GenerateSalvage))]
public static class Contract_GenerateSalvage_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions
            .MethodReplacer(
                AccessTools.Method(typeof(MechDef), nameof(MechDef.IsLocationDestroyed)),
                AccessTools.Method(typeof(Contract_GenerateSalvage_Patch), nameof(IsLocationDestroyed))
            )
            .MethodReplacer(
                AccessTools.Property(typeof(Pilot), "IsIncapacitated").GetGetMethod(),
                AccessTools.Method(typeof(Contract_GenerateSalvage_Patch), nameof(IsIncapacitated))
            );
    }

    private static MechDef? lastMechDef;
    public static bool IsLocationDestroyed(this MechDef mechDef, ChassisLocations location)
    {
        lastMechDef = mechDef;
        return mechDef.IsLocationDestroyed(location);
    }

    public static bool IsIncapacitated(this Pilot pilot)
    {
        try
        {
            if (!pilot.IsIncapacitated && lastMechDef != null && lastMechDef.HasDestroyedEngine())
            {
                return true;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return pilot.IsIncapacitated;
    }
}
