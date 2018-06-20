using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class MechStatisticsRulesCalculateMovementStatPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(MechStatisticsRulesCalculateMovementStatPatch), "MaxSprintDistance")
                    )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechDef), "get_Inventory"),
                    AccessTools.Method(typeof(MechStatisticsRulesCalculateMovementStatPatch), "Inventory")
                    );
        }

        private static MechDef def;

        public static void Prefix(MechDef mechDef)
        {
            def = mechDef;
        }

        public static void Postfix()
        {
            def = null;
        }

        public static float MaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            float walkSpeed = 0, runSpeed = 0, TTWalkSpeed = 0;
            Engine.CalculateMovementStat(def, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
            return runSpeed;
        }
        
        // disable jump jet calculations
        private static readonly MechComponentRef[] Empty = { };
        public static MechComponentRef[] Inventory(this MechDef @this)
        {
            return Empty;
        }
    }
}


