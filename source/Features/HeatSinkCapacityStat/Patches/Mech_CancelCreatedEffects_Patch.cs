﻿using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches
{
    [HarmonyPatch(typeof(Mech), nameof(Mech.CancelCreatedEffects))]
    public static class Mech_CancelCreatedEffects_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MechComponent), nameof(MechComponent.CancelCreatedEffects)),
                    AccessTools.Method(typeof(Mech_CancelCreatedEffects_Patch), nameof(CancelCreatedEffects))
                );
        }

        public static void CancelCreatedEffects(this MechComponent @this, bool performAuraRefresh)
        {
            if (@this.componentType == ComponentType.HeatSink)
            {
                return;
            }
            @this.CancelCreatedEffects(performAuraRefresh);
        }
    }
}