using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OverrideTonnage.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
    public static class MechLabMechInfoWidget_CalculateTonnage_Patch
    {
        private static MechDef mechDef;

        public static void Prefix(MechLabPanel ___mechLab)
        {
            mechDef = ___mechLab?.activeMechDef;
        }

        public static void Postfix()
        {
            mechDef = null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Property(typeof(ChassisDef), nameof(ChassisDef.InitialTonnage)).GetGetMethod(),
                    AccessTools.Method(typeof(MechLabMechInfoWidget_CalculateTonnage_Patch), nameof(OverrideInitialTonnage))
                );
        }

        public static float OverrideInitialTonnage(this ChassisDef chassisDef)
        {
            var tonnage = chassisDef.InitialTonnage;
            try
            {
                tonnage += OverrideTonnageFeature.Shared.TonnageChanges(mechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return tonnage;
        }
    }
}