using System;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch3
    {
        public static void Postfix(Mech __instance)
        {
            try
            {
                var mech = __instance;
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
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}