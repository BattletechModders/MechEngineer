using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    public class CirticalHitStatesHandler
    {
        public static readonly CirticalHitStatesHandler Shared = new CirticalHitStatesHandler();

        private CirticalHitStatesHandler()
        {
        }

        public bool ProcessWeaponHit(MechComponent mechComponent, CombatGameState combat, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects, List<MessageAddition> messages)
        {
            if (!(mechComponent.componentDef is ICriticalHitStates criticalHitStates))
            {
                return true;
            }

            if (criticalHitStates.CriticalHitStatesMaxCount <= 0)
            {
                return true;
            }

            if (damageLevel != ComponentDamageLevel.Destroyed) // only care about destructions, other things dont get through anyway
            {
                return true;
            }

            if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed) // already destroyed
            {
                return true;
            }

            if (!(mechComponent.parent is Mech))
            {
                return true;
            }

            var componentRef = mechComponent.mechComponentRef;
            var mech = (Mech) mechComponent.parent;
            var location = (ChassisLocations) mechComponent.Location;

            var crits = 1;
            var mechLocationDestroyed = mech.IsLocationDestroyed(location);
            if (mechLocationDestroyed)
            {
                crits = mechComponent.componentDef.InventorySize;
            }

            int critLevel;
            int oldCritLevel;
            {
                const string statisticName = "MechEngineer_CriticalHitState";

                var critStat = mechComponent.StatCollection.GetStatistic(statisticName) ?? mechComponent.StatCollection.AddStatistic(statisticName, 0);
                oldCritLevel = critStat.Value<int>();
                critLevel = oldCritLevel + crits;
                critStat.SetValue(critLevel);
            }

            {
                if (mechLocationDestroyed || critLevel > criticalHitStates.CriticalHitStatesMaxCount)
                {
                    damageLevel = ComponentDamageLevel.Destroyed;
                }
                else
                {
                    damageLevel = ComponentDamageLevel.Penalized;
                }

                mechComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);
            }

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                return true; // cancel effects, etc..
            }

            if (oldCritLevel > 0)
            {
                var hitEffects = criticalHitStates
                    .CriticalHitEffects
                    .Where(e => e.CriticalHitState == oldCritLevel);

                foreach (var hitEffect in hitEffects)
                {
                    var statusEffects = combat.EffectManager
                        .GetAllEffectsWithID(hitEffect.StatusEffect.Description.Id)
                        .Where(e => e.Target == mechComponent.parent);

                    foreach (var statusEffect in statusEffects)
                    {
                        mechComponent.parent.CancelEffect(statusEffect);
                    }
                }
            }

            {
                var hitEffects = criticalHitStates
                    .CriticalHitEffects
                    .Where(e => e.CriticalHitState == critLevel);

                foreach (var hitEffect in hitEffects)
                {
                    var effectData = hitEffect.StatusEffect;
                    if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive) // we only support passive for now
                    {
                        continue;
                    }

                    var text = $"PassiveEffect_{mechComponent.parent.GUID}_{mechComponent.uid}";
                    combat.EffectManager.CreateEffect(effectData, text, -1, mechComponent.parent, mechComponent.parent, default(WeaponHitInfo), 0, false);
                    mechComponent.createdEffectIDs.Add(text);
                }
            }

            {
                // this will also be called on engine crit, if side torso destroyed, does not necessarly mean CT is also detroyed
                var text = componentRef.Def.Description.UIName + " " + (crits == 1 ? " CRIT" : " CRIT X" + crits);
                messages.Add(new MessageAddition {Nature = FloatieMessage.MessageNature.ComponentDestroyed, Text = text});
            }
            return false;
        }
    }
}