using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMovementData")]
    public static class StatTooltipDataSetMovementDataPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxWalkDistance"),
                    AccessTools.Method(typeof(StatTooltipDataSetMovementDataPatch), "MaxWalkDistance")
                    )
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(StatTooltipDataSetMovementDataPatch), "MaxSprintDistance")
                    );
        }

        private static MechDef mechDef;
        private static float TTWalkSpeed;

        public static void Prefix(MechDef def)
        {
            mechDef = def;
        }

        public static void Postfix(StatTooltipData __instance)
        {
            mechDef = null;
            __instance.dataList.Add("TT Walk MP", string.Format("{0}", TTWalkSpeed));
        }

        public static float MaxWalkDistance(this MovementCapabilitiesDef @this)
        {
            float walkSpeed = 0, runSpeed = 0;
            Engine.CalculateMovementStat(mechDef, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
            return walkSpeed;
        }

        public static float MaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            float walkSpeed = 0, runSpeed = 0, TTWalkSpeed = 0;
            Engine.CalculateMovementStat(mechDef, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
            return runSpeed;
        }
    }

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


