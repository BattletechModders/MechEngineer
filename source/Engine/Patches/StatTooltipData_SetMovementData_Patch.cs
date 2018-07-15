using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMovementData")]
    public static class StatTooltipData_SetMovementData_Patch
    {
        private static MechDef mechDef;
        private static EngineMovement movement;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxWalkDistance"),
                    AccessTools.Method(typeof(StatTooltipData_SetMovementData_Patch), "OverrideMaxWalkDistance")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(StatTooltipData_SetMovementData_Patch), "OverrideMaxSprintDistance")
                );
        }

        public static void Prefix(MechDef def)
        {
            mechDef = def;
        }

        public static void Postfix(StatTooltipData __instance)
        {
            mechDef = null;
            if (movement != null)
            {
                __instance.dataList.Add("TT Walk MP", movement.MovementPoint.ToString());
                movement = null;
            }
        }

        public static float OverrideMaxWalkDistance(this MovementCapabilitiesDef @this)
        {
            try
            {
                movement = mechDef?.GetEngineMovement();
                if (movement != null)
                {
                    return movement.WalkSpeed;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.MaxWalkDistance;
        }

        public static float OverrideMaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            try
            {
                movement = mechDef?.GetEngineMovement();
                if (movement != null)
                {
                    return movement.RunSpeed;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.MaxSprintDistance;
        }
    }
}