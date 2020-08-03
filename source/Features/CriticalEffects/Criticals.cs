using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using FluffyUnderware.DevTools.Extensions;
using MechEngineer.Features.LocationalEffects;
using UnityEngine;

namespace MechEngineer.Features.CriticalEffects
{
    internal class Criticals
    {
        internal Criticals(MechComponent component)
        {
            this.component = component;
            ce = new Lazy<CriticalEffects>(FetchCriticalEffects);
        }

        private readonly MechComponent component;
        private AbstractActor actor => component.parent;
        
        internal CriticalEffects Effects => ce.Value;
        private readonly Lazy<CriticalEffects> ce;
        private bool HasLinked => Effects?.LinkedStatisticName != null;
        private CriticalEffects FetchCriticalEffects()
        { 
            var customs = component.componentDef.GetComponents<CriticalEffects>().ToList();

            if (actor is Mech)
            {
                var custom = customs.FirstOrDefault(x => x is MechCriticalEffects);
                if (custom != null)
                {
                    return custom;
                }
            }

            if (actor is Turret)
            {
                var custom = customs.FirstOrDefault(x => x is TurretCriticalEffects);
                if (custom != null)
                {
                    return custom;
                }
            }

            if (actor is Vehicle)
            {
                var custom = customs.FirstOrDefault(x => x is VehicleCriticalEffects);
                if (custom != null)
                {
                    return custom;
                }
            }

            {
                var custom = customs.FirstOrDefault(x => !(x is MechCriticalEffects) && !(x is TurretCriticalEffects) && !(x is VehicleCriticalEffects));
                return custom;
            }
        }

        internal void Hit(WeaponHitInfo hitInfo, ref ComponentDamageLevel damageLevel)
        {
            if (actor == null)
            {
                return;
            }

            SetHits(hitInfo, out damageLevel);
            
            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                PostDestructionEvents(hitInfo);
            }
        }

        private void SetHits(WeaponHitInfo hitInfo, out ComponentDamageLevel damageLevel)
        {
            // if max is reached, component is destroyed and no new effects can be applied
            // Destroyed components can still soak up crits, requires properly configured AIM from CAC
            var effectsMax = Effects?.PenalizedEffectIDs.Length + 1 ?? (component is Weapon ? 2 : 1);

            int effectsPrev, effectsNext;
            {
                var compCritsMax = ComponentHitMax();
                var compCritsPrev = ComponentHitCount();

                var locationDestroyed = actor.StructureForLocation(component.Location) <= 0f;
                var possibleAddedHits = locationDestroyed ? compCritsMax : 1;
                var compCritsNext = Mathf.Min(compCritsMax, compCritsPrev + possibleAddedHits);
                var compCritsAdded = Mathf.Max(compCritsNext - compCritsPrev, 0);

                ComponentHitCount(compCritsNext);

                // move to group/component abstraction, make sure that critsAdded is clear
                if (HasLinked)
                {
                    var prev = GroupHitCount();
                    var next = Mathf.Min(effectsMax, prev + compCritsAdded);
                    GroupHitCount(next);

                    effectsPrev = prev;
                    effectsNext = next;
                }
                else
                {
                    effectsPrev = compCritsPrev;
                    effectsNext = compCritsNext;
                }
            }
            
            damageLevel = effectsNext >= effectsMax ? ComponentDamageLevel.Destroyed : ComponentDamageLevel.Penalized;
            SetDamageLevel(hitInfo, damageLevel);
            
            if (Effects != null)
            {
                CancelEffects(effectsPrev, damageLevel);
                CreateEffects(effectsNext, damageLevel);
            }

            Control.mod.Logger.LogDebug(
                $"Component hit (uid={component.uid} Id={component.Description.Id} Location={component.Location}) " +
                $"effectsMax={effectsMax} effectsPrev={effectsPrev} effectsNext={effectsNext} " +
                $"damageLevel={damageLevel} HasEffects={Effects != null} LinkedStatisticName={Effects?.LinkedStatisticName}"
            );
        }

        public int ComponentHittableCount()
        {
            return ComponentHitMax() - ComponentHitCount();
        }

        private int ComponentHitMax()
        {
            // TODO fix size correctly for location:
            // introduce fake items for overflow location that is crit linked and overwrite component hit max for original + crit linked
            var additionalSize = component.componentDef.Is<DynamicSlots.DynamicSlots>(out var slot) && slot.InnerAdjacentOnly ? slot.ReservedSlots : 0;
            return component.componentDef.InventorySize + additionalSize;
        }

        private int ComponentHitCount(int? setHits = null)
        {
            var stat = component.StatCollection.MECriticalSlotsHit();
            stat.CreateIfMissing(); // move to Mech.init and remove "CreateIfMissing" from StatAdapter
            if (setHits.HasValue)
            {
                stat.Set(setHits.Value);
            }
            return stat.Get();
        }

        private int GroupHitCount(int? setHits = null)
        {
            var statisticName = LinkedScopedId();
            var collection = actor.StatCollection;

            var critStat = collection.GetStatistic(statisticName) ?? collection.AddStatistic(statisticName, 0);
            if (setHits.HasValue)
            {
                critStat.SetValue(setHits.Value);
            }
            return critStat?.Value<int>() ?? 0;
        }

        private string LinkedScopedId()
        {
            return ScopedId(Effects.LinkedStatisticName);
        }

        private string ScopedId(string id)
        {
            if (LocationNaming.Localize(id, component, out var localizedId))
            {
                return localizedId;
            }

            if (!HasLinked)
            {
                var uid = component.uid;
                return $"{id}_{uid}";
            }

            return id;
        }

        private void SetDamageLevel(WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            SetDamageLevel(component, hitInfo, damageLevel);

            if (HasLinked)
            {
                var id = LinkedScopedId();

                //Control.mod.Logger.LogDebug($"HasLinked scopeId={scopedId}");
                foreach (var otherMechComponent in actor.allComponents)
                {
                    if (otherMechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
                    {
                        continue;
                    }

                    var otherCriticals = otherMechComponent.Criticals();
                    if (!otherCriticals.HasLinked)
                    {
                        continue;
                    }

                    var otherId = otherCriticals.LinkedScopedId();
                    if (id == otherId)
                    {
                        SetDamageLevel(otherMechComponent, hitInfo, damageLevel);
                    }
                }
            }

            static void SetDamageLevel(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
            {
                //Control.mod.Logger.LogDebug($"damageLevel={damageLevel} uid={mechComponent.uid} (Id={mechComponent.Description.Id} Location={mechComponent.Location})");
                mechComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);

                if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    mechComponent.CancelCreatedEffects();
                }
            }
        }

        private void CancelEffects(int critsPrev, ComponentDamageLevel damageLevel)
        {
            var effectIdsIndex = critsPrev - 1;

            var effectIds = new string[0];
            if (effectIdsIndex >= 0 && effectIdsIndex < Effects.PenalizedEffectIDs.Length)
            {
                effectIds = effectIds.AddRange(Effects.PenalizedEffectIDs[effectIdsIndex]);
            }

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                effectIds = effectIds.AddRange(Effects.OnDestroyedDisableEffectIds);
            }

            foreach (var effectId in effectIds)
            {
                var resolvedEffectId = ScopedId(effectId);
                var util = new EffectIdUtil(effectId, resolvedEffectId, component);
                util.CancelCriticalEffect();
            }
        }

        private void CreateEffects(int critsNext, ComponentDamageLevel damageLevel)
        {
            var effectIdsIndex = critsNext - 1;

            string[] effectIds;
            if (effectIdsIndex >= 0 && effectIdsIndex < Effects.PenalizedEffectIDs.Length)
            {
                effectIds = Effects.PenalizedEffectIDs[effectIdsIndex];
            }
            else if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                effectIds = Effects.OnDestroyedEffectIDs;
            }
            else
            {
                effectIds = new string[0];
            }

            // collect disabled effects, probably easier to cache these in a mech statistic
            var disabledScopedEffectIds = DisabledSimpleScopedEffectIdsOnActor(actor);
            //Control.mod.Logger.LogDebug($"disabledEffectIds={string.Join(",", disabledEffectIds.ToArray())}");
            foreach (var effectId in effectIds)
            {
                var simpleScopedEffectId = ScopedId(effectId);
                if (disabledScopedEffectIds.Contains(simpleScopedEffectId))
                {
                    continue;
                }

                var resolvedEffectId = ScopedId(effectId);
                var util = new EffectIdUtil(effectId, resolvedEffectId, component);
                util.CreateCriticalEffect(damageLevel < ComponentDamageLevel.Destroyed);
            }
            
            static HashSet<string> DisabledSimpleScopedEffectIdsOnActor(AbstractActor actor)
            {
                var iter = from mc in actor.allComponents
                    where !mc.IsFunctional
                    let ce = mc.Criticals().Effects
                    where ce != null
                    from effectId in ce.OnDestroyedDisableEffectIds
                    select mc.Criticals().ScopedId(effectId);

                return new HashSet<string>(iter);
            }
        }

        private void PostDestructionEvents(WeaponHitInfo hitInfo)
        {
            if (Effects == null)
            {
                return;
            }

            if (Effects.DeathMethod != DeathMethod.NOT_SET)
            {
                actor.FlagForDeath(
                    $"{component.UIName} DESTROYED",
                    Effects.DeathMethod,
                    DamageType.Combat,
                    component.Location,
                    hitInfo.stackItemUID,
                    hitInfo.attackerId,
                    false);
                actor.HandleDeath(hitInfo.attackerId);
            }

            if (!string.IsNullOrEmpty(Effects.OnDestroyedVFXName))
            {
                actor.GameRep.PlayVFX(component.Location, Effects.OnDestroyedVFXName, true, Vector3.zero, true, -1f);
            }

            if (!string.IsNullOrEmpty(Effects.OnDestroyedAudioEventName))
            {
                WwiseManager.PostEvent(Effects.OnDestroyedAudioEventName, actor.GameRep.audioObject);
            }
        }
    }

    internal class EffectIdUtil
    {
        private readonly string templateEffectId;
        private readonly string resolvedEffectId;
        private readonly MechComponent mechComponent;

        internal EffectIdUtil(string templateEffectId, string resolvedEffectId, MechComponent mechComponent)
        {
            this.templateEffectId = templateEffectId;
            this.resolvedEffectId = $"MECriticalHitEffect_{resolvedEffectId}_{mechComponent.parent.GUID}";
            this.mechComponent = mechComponent;
        }

        internal static void CreateEffect(MechComponent component, EffectData effectData, string effectId, bool tracked = true)
        {
            var actor = component.parent;
            
            Control.mod.Logger.LogDebug($"Creating id={effectId} statName={effectData.statisticData.statName}");
            actor.Combat.EffectManager.CreateEffect(effectData, effectId, -1, actor, actor, default, 0);

            if (tracked)
            {
                // make sure created effects are removed once component got destroyed
                // don't track if you want an effect to persist even after destruction (e.g. critical hit effects)
                component.createdEffectIDs.Add(effectId);
            }
        }
        
        internal void CreateCriticalEffect(bool tracked = true)
        {
            var effectData = CriticalEffectsFeature.GetEffectData(templateEffectId);
            if (effectData == null)
            {
                return;
            }
            if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive) // we only support passive for now
            {
                return;
            }

            LocationalEffectsFeature.ProcessLocationalEffectData(ref effectData, mechComponent);

            CreateEffect(mechComponent, effectData, resolvedEffectId, tracked);
        }

        internal void CancelCriticalEffect()
        {
            var actor = mechComponent.parent;
            var statusEffects = actor.Combat.EffectManager
                .GetAllEffectsWithID(resolvedEffectId)
                .Where(e => e.Target == actor);

            Control.mod.Logger.LogDebug($"Canceling id={resolvedEffectId}");
            foreach (var statusEffect in statusEffects)
            {
                Control.mod.Logger.LogDebug($"Canceling statName={statusEffect.EffectData.statisticData.statName}");
                actor.CancelEffect(statusEffect);
            }
            mechComponent.createdEffectIDs.Remove(resolvedEffectId);
        }
    }

    internal static class StatCollectionExtension
    {
        internal static StatisticAdapter<int> MECriticalSlotsHit(this StatCollection statCollection)
        {
            return new StatisticAdapter<int>("MECriticalSlotsHit", statCollection, 0);
        }
    }
}