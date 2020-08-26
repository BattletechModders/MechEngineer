using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
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
            if (ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled)
            {
                heat.ShutdownCausesInjury = receiveShutdownInjury;
            }
            return heat;
        }

        private static bool receiveShutdownInjury = true;
        public static void Prefix(MechShutdownSequence __instance)
        {
            try
            {
                if (!ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled)
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
                receiveShutdownInjury = mech.StatCollection.ReceiveShutdownInjury().Get();
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        public static void Postfix()
        {
            receiveShutdownInjury = false;
        }
    }
}
