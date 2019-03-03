using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), nameof(MechComponent.InitPassiveSelfEffects))]
    public static class MechComponent_InitPassiveSelfEffects_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MechComponent), nameof(ApplyPassiveEffectToTarget)),
                AccessTools.Method(typeof(MechComponent_InitPassiveSelfEffects_Patch), nameof(ApplyPassiveEffectToTarget))
            );
        }
        
        public static void ApplyPassiveEffectToTarget(
            this MechComponent mechComponent,
            EffectData effect,
            AbstractActor creator,
            ICombatant target,
            string effectID)
        {
            try
            {
                LocationalEffects.ProcessLocationalEffectData(effect, mechComponent);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            
            var effectManager = mechComponent.parent.Combat.EffectManager;
            effectManager.CreateEffect(
                effect, effectID, 
                -1, 
                mechComponent.parent, target, 
                new WeaponHitInfo(), 0, false);
        }
    }
}
