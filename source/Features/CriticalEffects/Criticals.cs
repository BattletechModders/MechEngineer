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
        private readonly MechComponent component;

        private readonly Lazy<CriticalEffects> ce;
        internal CriticalEffects Effects => ce.Value;

        internal Criticals(MechComponent component)
        {
            this.component = component;
            ce = new Lazy<CriticalEffects>(FetchCriticalEffects);
        }

        internal void Hit(WeaponHitInfo hitInfo, ref ComponentDamageLevel damageLevel)
        {
            if (component.parent == null)
            {
                return;
            }

            if (component.DamageLevel == ComponentDamageLevel.Destroyed) // already destroyed
            {
                return;
            }

            if (Effects == null)
            {
                return;
            }

            var actor = component.parent;

            int critsMax, critsPrev, critsNext;
            {
                var compCritsMax = ComponentHitMax();
                var compCritsPrev = ComponentHitCount();

                var locationDestroyed = actor.StructureForLocation(component.Location) <= 0f;
                var possibleAddedHits = locationDestroyed ? compCritsMax : 1;
                var compCritsNext = Mathf.Min(compCritsMax, compCritsPrev + possibleAddedHits);
                var compCritsAdded = Mathf.Max(compCritsNext - compCritsPrev, 0);

                // move to group/component abstraction, make sure that critsAdded is clear
                if (HasLinked)
                {
                    var prev = GroupHitCount();
                    var max = GroupHitMax();
                    var next = Mathf.Min(max, prev + compCritsAdded);
                    GroupHitCount(next);

                    critsMax = max;
                    critsPrev = prev;
                    critsNext = next;
                }
                else
                {
                    critsMax = compCritsMax;
                    critsPrev = compCritsPrev;
                    critsNext = compCritsNext;
                }
                ComponentHitCount(compCritsNext);
            }


            { // move to group/component abstraction
                damageLevel = critsNext >= critsMax ? ComponentDamageLevel.Destroyed : ComponentDamageLevel.Penalized;

                SetDamageLevel(component, hitInfo, damageLevel);
            
                if (HasLinked)
                {
                    var scopedId = LinkedScopedId();
                
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
                    
                        var otherScopedId = otherCriticals.LinkedScopedId();
                        if (scopedId == otherScopedId)
                        {
                            SetDamageLevel(otherMechComponent, hitInfo, damageLevel);
                        }
                    }
                }
            }
            
            CancelEffects(critsPrev, damageLevel);
            CreateEffects(critsNext, damageLevel, actor);

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
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

            Control.mod.Logger.LogDebug(
                $"Component hit (uid={component.uid} Id={component.Description.Id} Location={component.Location}) " +
                $"critsMax={critsMax} critsPrev={critsPrev} critsNext={critsNext} " +
                $"damageLevel={damageLevel} " +
                $"HasLinked={HasLinked}"
            );
        }

        private bool HasLinked => Effects?.LinkedStatisticName != null;

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

        private void CreateEffects(int critsNext, ComponentDamageLevel damageLevel, AbstractActor actor)
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
        }

        private static HashSet<string> DisabledSimpleScopedEffectIdsOnActor(AbstractActor actor)
        {
            var iter = from mc in actor.allComponents
                where !mc.IsFunctional
                let ce = mc.Criticals().Effects
                where ce != null
                from effectId in ce.OnDestroyedDisableEffectIds
                select mc.Criticals().ScopedId(effectId);

            return new HashSet<string>(iter);
        }

        private static void SetDamageLevel(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            //Control.mod.Logger.LogDebug($"damageLevel={damageLevel} uid={mechComponent.uid} (Id={mechComponent.Description.Id} Location={mechComponent.Location})");
            mechComponent.StatCollection.ModifyStat(
                hitInfo.attackerId,
                hitInfo.stackItemUID,
                "DamageLevel",
                StatCollection.StatOperation.Set,
                damageLevel);
        }

        private const string HitsStatisticName = "MECriticalSlotsHit";

        public int ComponentHittableCount()
        {
            return ComponentHitMax() - ComponentHitCount();
        }

        private int ComponentHitMax()
        {
            return component.componentDef.InventorySize;
        }

        private int ComponentHitCount(int? setHits = null)
        {
            var ce = Effects;
            if (ce == null)
            {
                switch (component.DamageLevel)
                {
                    case ComponentDamageLevel.Destroyed:
                        return ComponentHitMax();
                    case ComponentDamageLevel.Penalized:
                        return 1;
                    default:
                        return 0;
                }
            }

            var statisticName = HitsStatisticName;
            var collection = component.StatCollection;
            var critStat = collection.GetStatistic(HitsStatisticName);
            if (setHits.HasValue)
            {
                if (critStat == null)
                {
                    critStat = collection.AddStatistic(statisticName, setHits.Value);
                }
                else
                {
                    critStat.SetValue(setHits.Value);
                }
            }

            return critStat?.Value<int>() ?? 0;
        }

        private int GroupHitMax()
        {
            return Effects.PenalizedEffectIDs.Length;
        }

        private int GroupHitCount(int? setHits = null)
        {
            var statisticName = LinkedScopedId();
            var collection = component.parent.StatCollection;

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

        private CriticalEffects FetchCriticalEffects() { 
            var customs = component.componentDef.GetComponents<CriticalEffects>().ToList();

            if (component.parent is Mech)
            {
                var custom = customs.FirstOrDefault(x => x is MechCriticalEffects);
                if (custom != null)
                {
                    return custom;
                }
            }

            if (component.parent is Turret)
            {
                var custom = customs.FirstOrDefault(x => x is TurretCriticalEffects);
                if (custom != null)
                {
                    return custom;
                }
            }

            if (component.parent is Vehicle)
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
            
            var actor = mechComponent.parent;

            LocationalEffectsFeature.ProcessLocationalEffectData(ref effectData, mechComponent);
            
            Control.mod.Logger.LogDebug($"Creating id={resolvedEffectId} statName={effectData.statisticData.statName}");
            actor.Combat.EffectManager.CreateEffect(effectData, resolvedEffectId, -1, actor, actor, default, 0);

            //DebugUtils.LogActor("CreateCriticalEffect", actor);

            if (tracked)
            {
                // make sure created effects are removed once component got destroyed
                mechComponent.createdEffectIDs.Add(resolvedEffectId);
            }
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
}