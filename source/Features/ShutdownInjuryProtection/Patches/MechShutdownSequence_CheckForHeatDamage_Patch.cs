using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechShutdownSequence), "CheckForHeatDamage")]
    public static class MechShutdownSequence_CheckForHeatDamage_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(CombatGameConstants), "get_Heat"),
                AccessTools.Method(typeof(MechShutdownSequence_CheckForHeatDamage_Patch), "OverrideHeat")
            );
        }

        public static HeatConstantsDef OverrideHeat(this CombatGameConstants @this)
        {
            var heat = @this.Heat;
            heat.ShutdownCausesInjury = overrideShutdownCausesInjury || heat.ShutdownCausesInjury;
            return heat;
        }

        private static bool overrideShutdownCausesInjury = false;
        public static void Prefix(MechShutdownSequence __instance)
        {
            try
            {
                var traverse = Traverse.Create(__instance);
                var combat = traverse.Property("Combat").GetValue<CombatGameState>();
                if (combat.Constants.Heat.ShutdownCausesInjury)
                {
                    return;
                }

                var mech = traverse.Property("OwningMech").GetValue<Mech>();
                overrideShutdownCausesInjury = !CockpitHandler.Shared.ProtectsAgainstShutdownInjury(mech.MechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix()
        {
            overrideShutdownCausesInjury = false;
        }
    }
}