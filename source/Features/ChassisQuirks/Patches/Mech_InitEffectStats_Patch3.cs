using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitStats")]
    public static class Mech_InitEffectStats_Patch3
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Property(typeof(AbstractActor), nameof(AbstractActor.SummaryArmorCurrent)).GetGetMethod(),
                    AccessTools.Method(typeof(Mech_InitEffectStats_Patch3), nameof(OverrideSummaryArmorCurrent))
                );
        }

        // this is called before components InitEffects
        //public static void Postfix(Mech __instance)
        public static float OverrideSummaryArmorCurrent(this AbstractActor mech)
        {
            try
            {
                ChassisInitEffects(mech as Mech);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return mech.SummaryArmorCurrent;
        }

        private static void ChassisInitEffects(Mech mech)
        {
            if (!mech.MechDef.Chassis.Is<ChassisQuirks>(out var quirks))
            {
                return;
            }
            
            if (quirks.statusEffects == null)
            {
                return;
            }

            foreach (var effectData in quirks.statusEffects)
            {
                if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive)
                {
                    continue;
                }

                var effectID = $"ChassisEffect_{effectData.Description.Id}_{mech.GUID}";
                mech.Combat.EffectManager.CreateEffect(effectData, effectID, -1, mech, mech, default(WeaponHitInfo), 0);

                //Control.mod.Logger.LogDebug($"Adding {effectID} to {mech.MechDef.Chassis.Description.Id}!!!");
            }
        }
    }
}