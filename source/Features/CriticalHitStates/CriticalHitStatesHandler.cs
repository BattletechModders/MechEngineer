using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using Localize;
using UnityEngine;

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
                // TODO track hits on same component
                // TODO already doing it
                crits = mechComponent.componentDef.InventorySize;
            }

            int critLevel;
            int oldCritLevel;
            {
                const string statisticName = "MechEngineer_CriticalHitState";

                var critStat = mechComponent.StatCollection.GetStatistic(statisticName) ?? mechComponent.StatCollection.AddStatistic(statisticName, 0);
                oldCritLevel = critStat.Value<int>();
                critLevel = Mathf.Max(oldCritLevel + crits, criticalHitStates.MaxStates + 1);
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
                    mechComponent.CancelCriticalEffect(hitEffect.StatusEffect);
                }
            }

            {
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

                    if (damageLevel < ComponentDamageLevel.Destroyed)
                    {
                        mechComponent.CreateCriticalEffect(effectData);
                    }
                    else
                    {
                        mechComponent.CreateCriticalEffect(effectData, false);
                    }
                }
            }

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                var text = new Text("{0} DESTROYED", mechComponent.UIName);
                if (criticalHitStates.DeathMethod != DeathMethod.NOT_SET)
                {
                    mech.FlagForDeath(
                        text.ToString(),
                        criticalHitStates.DeathMethod,
                        DamageType.Combat,
                        mechComponent.Location,
                        hitInfo.stackItemUID,
                        hitInfo.attackerId,
                        false);
                }
                else
                {
                    mechComponent.PublishMessage(text, FloatieMessage.MessageNature.ComponentDestroyed);
                }
            }
            else
            {
                mechComponent.PublishMessage(
                    new Text("{0} " + (crits == 1 ? "CRIT" : "CRIT X" + crits), mechComponent.UIName),
                    FloatieMessage.MessageNature.CriticalHit
                );
            }

            return false;
        }
    }

    internal static class EffectDataExtensions
    {   
        internal static void CreateCriticalEffect(this MechComponent mechComponent, EffectData effectData, bool tracked = true)
        {
            var effectId = mechComponent.EffectId(effectData);
            
            var actor = mechComponent.parent;
            
            actor.Combat.EffectManager.CreateEffect(
                effectData, effectId, -1,
                actor, actor,
                default(WeaponHitInfo), 0, false);

            if (tracked)
            {
                // make sure created effects are removed once component got destroyed
                mechComponent.createdEffectIDs.Add(effectId);
            }
        }

        internal static void CancelCriticalEffect(this MechComponent mechComponent, EffectData effectData)
        {
            var effectId = mechComponent.EffectId(effectData);

            var actor = mechComponent.parent;
            var statusEffects = actor.Combat.EffectManager
                .GetAllEffectsWithID(effectId)
                .Where(e => e.Target == actor);

            foreach (var statusEffect in statusEffects)
            {
                actor.CancelEffect(statusEffect);
                mechComponent.createdEffectIDs.Remove(effectId);
            }
        }
        
        private static string EffectId(this MechComponent mechComponent, EffectData statusEffect)
        {
            return $"{mechComponent.parent.EffectId(statusEffect)}_{mechComponent.uid}";
        }
        
        private static string EffectId(this AbstractActor actor, EffectData statusEffect)
        {
            return $"CriticalHitEffect_{statusEffect.Description.Id}_{actor.GUID}";
        } 
    }
}