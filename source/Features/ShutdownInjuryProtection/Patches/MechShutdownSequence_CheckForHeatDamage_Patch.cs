using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches
{
    [HarmonyPatch(typeof(MechShutdownSequence), nameof(MechShutdownSequence.CheckForHeatDamage))]
    public static class MechShutdownSequence_CheckForHeatDamage_Patch
    {
        public static bool Prepare()
        {
            return !ShutdownInjuryProtectionFeature.settings.ShutdownInjuryEnabled;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(CombatGameConstants), "get_Heat"),
                AccessTools.Method(typeof(MechShutdownSequence_CheckForHeatDamage_Patch), nameof(OverrideHeat))
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
                if (__instance.Combat.Constants.Heat.ShutdownCausesInjury)
                {
                    return;
                }

                var mech = __instance.OwningMech;
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