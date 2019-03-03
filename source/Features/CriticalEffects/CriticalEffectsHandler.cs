using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using FluffyUnderware.DevTools.Extensions;
using Localize;
using UnityEngine;

namespace MechEngineer
{
    public class CriticalEffectsHandler
    {
        private static Dictionary<string, EffectData> Settings { get; set; } = new Dictionary<string, EffectData>();

        internal static void Setup(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            Settings = SettingsResourcesTools.Enumerate<EffectData>("MECriticalEffects", customResources)
                .ToDictionary(entry => entry.Description.Id);
        }
        
        public static readonly CriticalEffectsHandler Shared = new CriticalEffectsHandler();

        private CriticalEffectsHandler()
        {
        }

        public bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            var criticalEffects = mechComponent.componentDef?.GetComponent<CriticalEffects>();
            if (criticalEffects == null)
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

            damageLevel = ComponentDamageLevel.Penalized;

            var location = mechComponent.mechComponentRef.MountedLocation;
            var mechLocationDestroyed = mech.IsLocationDestroyed(location);
            
            int critsPrev;
            int critsNext;
            int critsAdded;
            {
                critsPrev = criticalEffects.HasLinked ? mechComponent.CriticalSlotsHitLinked() : mechComponent.CriticalSlotsHit();
                var critsMax = criticalEffects.PenalizedEffectIDs.Length + 1; // max = how many crits can be absorbed, last one destroys component

                var slots = mechComponent.CriticalSlots(); // critical slots left

                var critsHit = mechLocationDestroyed ? slots : Mathf.Min(1, slots);

                critsNext = Mathf.Min(critsMax, critsPrev + critsHit);
                if (critsNext >= critsMax)
                {
                    damageLevel = ComponentDamageLevel.Destroyed;
                }
                if (criticalEffects.HasLinked)
                {
                    mechComponent.CriticalSlotsHitLinked(critsNext);
                }

                critsAdded = Mathf.Max(critsNext - critsPrev, 0);
                
                var slotsHitPrev = mechComponent.CriticalSlotsHit();
                mechComponent.CriticalSlotsHit(slotsHitPrev + critsAdded);

                Control.mod.Logger.LogDebug(
                    $"{criticalEffects.Def.Description.Id} on {mechComponent.mechComponentRef.MountedLocation} " +
                    $"critsAdded={critsAdded} critsMax={critsMax} " +
                    $"critsPrev={critsPrev} critsNext={critsNext} " +
                    $"critsHit={critsHit} " +
                    $"slots={slots} slotsHitPrev={slotsHitPrev} " +
                    $"damageLevel={damageLevel} " +
                    $"HasLinked={criticalEffects.HasLinked}"
                );
            }

            {
                SetDamageLevel(mechComponent, hitInfo, damageLevel);
            
                if (criticalEffects.HasLinked)
                {
                    var scopedId = mechComponent.ScopedId(criticalEffects.LinkedStatisticName, criticalEffects.Scope);
                
                    foreach (var mc in mech.allComponents)
                    {
                        var r = mc.mechComponentRef;
                        if (r.DamageLevel == ComponentDamageLevel.Destroyed)
                        {
                            continue;
                        }

                        if (!r.Is<CriticalEffects>(out var ce))
                        {
                            continue;
                        }
                        
                        if (string.IsNullOrEmpty(ce.LinkedStatisticName))
                        {
                            continue;
                        }
                    
                        var otherScopedId = mc.ScopedId(ce.LinkedStatisticName, ce.Scope);
                        if (scopedId == otherScopedId)
                        {
                            SetDamageLevel(mc, hitInfo, damageLevel);
                        }
                    }
                }
            }

            {
                // cancel effects
                var effectIds = new string[0];
                
                if (critsPrev > 0 && critsPrev <= criticalEffects.PenalizedEffectIDs.Length)
                {
                    effectIds = effectIds.AddRange(criticalEffects.PenalizedEffectIDs[critsPrev - 1]);
                }
                
                if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    effectIds = effectIds.AddRange(criticalEffects.OnDestroyedDisableEffectIds);
                }
                
                foreach (var effectId in effectIds)
                {
                    var util = new EffectIdUtil(effectId, mechComponent, criticalEffects.Scope);
                    util.CancelCriticalEffect();
                }
            }

            {
                // create effects
                var effectIds = new string[0];
                
                if (critsNext > 0 && critsNext <= criticalEffects.PenalizedEffectIDs.Length)
                {
                    effectIds = criticalEffects.PenalizedEffectIDs[critsNext - 1];
                }
                
                if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    effectIds = criticalEffects.OnDestroyedEffectIDs;
                }
                
                // collect disabled effects, probably easier to cache these in a mech statistic
                var disabledEffectIds = DisabledScopedIdsOnMech(mech);

                foreach (var effectId in effectIds)
                {
                    var scopedId = mechComponent.ScopedId(effectId, criticalEffects.Scope);
                    if (disabledEffectIds.Contains(scopedId))
                    {
                        continue;
                    }
                    
                    var util = new EffectIdUtil(effectId, mechComponent, criticalEffects.Scope);
                    util.CreateCriticalEffect(damageLevel < ComponentDamageLevel.Destroyed);
                }
            }

            if (damageLevel == ComponentDamageLevel.Destroyed)
            {
                var text = new Text("{0} DESTROYED", mechComponent.UIName);
                if (criticalEffects.DeathMethod != DeathMethod.NOT_SET)
                {
                    mech.FlagForDeath(
                        text.ToString(),
                        criticalEffects.DeathMethod,
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
                    new Text("{0} " + (critsAdded == 1 ? "CRIT" : "CRIT X" + critsAdded), mechComponent.UIName),
                    FloatieMessage.MessageNature.CriticalHit
                );
            }

            return false;
        }

        private static void SetDamageLevel(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            mechComponent.StatCollection.ModifyStat(
                hitInfo.attackerId,
                hitInfo.stackItemUID,
                "DamageLevel",
                StatCollection.StatOperation.Set,
                damageLevel);
        }

        private static HashSet<string> DisabledScopedIdsOnMech(Mech mech)
        {
            var disabledEffectIds = new HashSet<string>();
            
            foreach (var mc in mech.allComponents)
            {
                var r = mc.mechComponentRef;
                if (r.DamageLevel != ComponentDamageLevel.Destroyed)
                {
                    continue;
                }

                if (!r.Is<CriticalEffects>(out var ce))
                {
                    continue;
                }

                foreach (var effectId in ce.OnDestroyedDisableEffectIds)
                {
                    var scopedId = mc.ScopedId(effectId, ce.Scope);
                    disabledEffectIds.Add(scopedId);
                }
            }

            return disabledEffectIds;
        }
        
        private class EffectIdUtil
        {
            private string effectId;
            private MechComponent mechComponent;
            private CriticalEffects.ScopeEnum scope;
    
            internal EffectIdUtil(string effectId, MechComponent mechComponent, CriticalEffects.ScopeEnum scope)
            {
                this.effectId = effectId;
                this.mechComponent = mechComponent;
                this.scope = scope;
            }
            
            internal void CreateCriticalEffect(bool tracked = true)
            {
                var scopedId = ScopedId();
                
                if (!Settings.TryGetValue(effectId, out var effectData))
                {
                    Control.mod.Logger.LogError($"Can't find critical effect id '{effectId}'");
                    return;
                }
                if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive) // we only support passive for now
                {
                    return;
                }
                
                var actor = mechComponent.parent;

                effectData = LocationalEffects.ProcessLocationalEffectData(effectData, mechComponent);
                
                Control.mod.Logger.LogDebug($"Creating scopedId={scopedId} statName={effectData.statisticData.statName}");
                actor.Combat.EffectManager.CreateEffect(
                    effectData, scopedId, -1,
                    actor, actor,
                    default(WeaponHitInfo), 0, false);
    
                if (tracked)
                {
                    // make sure created effects are removed once component got destroyed
                    mechComponent.createdEffectIDs.Add(scopedId);
                }
            }
    
            internal void CancelCriticalEffect()
            {
                var scopedId = ScopedId();
    
                var actor = mechComponent.parent;
                var statusEffects = actor.Combat.EffectManager
                    .GetAllEffectsWithID(scopedId)
                    .Where(e => e.Target == actor);
    
                Control.mod.Logger.LogDebug($"Canceling scopedId={scopedId}");
                foreach (var statusEffect in statusEffects)
                {
                    Control.mod.Logger.LogDebug($"Canceling statName={statusEffect.EffectData.statisticData.statName}");
                    actor.CancelEffect(statusEffect);
                }
                mechComponent.createdEffectIDs.Remove(scopedId);
            }
            
            private string ScopedId()
            {
                var id = $"MECriticalHitEffect_{effectId}_{mechComponent.parent.GUID}";
                return mechComponent.ScopedId(id, scope);
            }
        }
    }
}