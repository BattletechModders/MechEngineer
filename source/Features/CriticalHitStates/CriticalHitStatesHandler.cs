using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    public class CirticalHitStatesHandler
    {
        public static readonly CirticalHitStatesHandler Shared = new CirticalHitStatesHandler();

        private CirticalHitStatesHandler()
        {
        }

        public bool ProcessWeaponHit(
            MechComponent mechComponent,
            CombatGameState combat,
            WeaponHitInfo hitInfo,
            ComponentDamageLevel damageLevel,
            bool applyEffects,
            List<MessageAddition> messages,
            CriticalHitStates criticalHitStates = null,
            int? hits = null)
        {
            if (criticalHitStates == null)
            {
                criticalHitStates = mechComponent.componentDef?.GetComponent<CriticalHitStates>();
            }

            if (criticalHitStates == null)
            {
                return true;
            }

            if (criticalHitStates.MaxStates <= 0)
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

            if (!(mechComponent.parent is Mech mech))
            {
                return true;
            }

            var componentRef = mechComponent.mechComponentRef;
            var location = (ChassisLocations) mechComponent.Location;
            var mechLocationDestroyed = mech.IsLocationDestroyed(location);

            var crits = 1;
            if (hits.HasValue)
            {
                crits = hits.Value;
            }
            else if (mechLocationDestroyed)
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
                if (mechLocationDestroyed || critLevel > criticalHitStates.MaxStates)
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

            //Control.mod.Logger.LogDebug($"oldCritLevel={oldCritLevel} critLevel={critLevel} damageLevel={damageLevel}");

            if (oldCritLevel > 0)
            {
                var hitEffects = criticalHitStates
                    .HitEffects
                    .Where(e => e.State == oldCritLevel);

                foreach (var hitEffect in hitEffects)
                {
                    var effectId = hitEffect.StatusEffect.IdForComponent(mechComponent);

                    var statusEffects = combat.EffectManager
                        .GetAllEffectsWithID(effectId)
                        .Where(e => e.Target == mechComponent.parent);

                    foreach (var statusEffect in statusEffects)
                    {
                        mechComponent.parent.CancelEffect(statusEffect);
                        mechComponent.createdEffectIDs.Remove(effectId);
                    }
                }
            }

            if (damageLevel < ComponentDamageLevel.Destroyed) {
                var hitEffects = criticalHitStates
                    .HitEffects
                    .Where(e => e.State == critLevel);

                foreach (var hitEffect in hitEffects)
                {
                    var effectData = hitEffect.StatusEffect;
                    if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive) // we only support passive for now
                    {
                        continue;
                    }

                    var effectId = hitEffect.StatusEffect.IdForComponent(mechComponent);
                    combat.EffectManager.CreateEffect(effectData, effectId, -1, mechComponent.parent, mechComponent.parent, default(WeaponHitInfo), 0, false);
                    mechComponent.createdEffectIDs.Add(effectId);
                }
            }

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                if (criticalHitStates.DeathMethod != DeathMethod.NOT_SET)
                {
                    mech.FlagForDeath(
                        mechComponent.Description.Name + " DESTROYED",
                        criticalHitStates.DeathMethod,
                        mechComponent.Location,
                        hitInfo.stackItemUID,
                        hitInfo.attackerId,
                        false);
                }
                else
                {
                    var text = componentRef.Def.Description.UIName + " DESTROYED";
                    messages.Add(new MessageAddition {Nature = FloatieMessage.MessageNature.ComponentDestroyed, Text = text});
                }
            }
            else
            {
                var text = componentRef.Def.Description.UIName + " " + (crits == 1 ? " CRIT" : " CRIT X" + crits);
                messages.Add(new MessageAddition {Nature = FloatieMessage.MessageNature.ComponentDestroyed, Text = text});
            }

            return false;
        }
    }

    internal static class EffectDataExtensions
    {
        internal static string IdForComponent(this EffectData statusEffect, MechComponent mechComponent)
        {
            return $"{statusEffect.Description.Id}_{mechComponent.parent.GUID}_{mechComponent.uid}";
        } 
    }
}