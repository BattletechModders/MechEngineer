using System;
using System.Collections.Generic;
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
            if (Control.settings.ShutdownInjuryEnabled)
            {
                heat.ShutdownCausesInjury = !protectedAgainstShutdownInjury;
            }
            return heat;
        }

        private static bool protectedAgainstShutdownInjury = true;
        public static void Prefix(MechShutdownSequence __instance)
        {
            try
            {
                if (!Control.settings.ShutdownInjuryEnabled)
                {
                    return;
                }

                var traverse = Traverse.Create(__instance);
                var combat = traverse.Property("Combat").GetValue<CombatGameState>();
                if (combat.Constants.Heat.ShutdownCausesInjury)
                {
                    return;
                }

                var mech = traverse.Property("OwningMech").GetValue<Mech>();
                var stat = mech.StatCollection.ProtectsAgainstShutdownInjury();
                protectedAgainstShutdownInjury = stat != null && stat.Value<bool>();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix()
        {
            protectedAgainstShutdownInjury = false;
        }
    }
}
