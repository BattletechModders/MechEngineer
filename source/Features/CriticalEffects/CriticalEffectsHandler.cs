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

            var location = mechComponent.mechComponentRef.MountedLocation;
            var mechLocationDestroyed = mech.IsLocationDestroyed(location);
            
            int critLevel;
            int oldCritLevel;
            int crits;
            {
                {
                    const string statisticName = "MECriticalHits";

                    var collection = mechComponent.StatCollection;
                    var critStat = collection.GetStatistic(statisticName) ?? collection.AddStatistic(statisticName, 0);
                    oldCritLevel = critStat.Value<int>();
                    var maxCrits = mechComponent.componentDef.InventorySize;
                    if (mechLocationDestroyed)
                    {
                        crits = Mathf.Max(maxCrits - oldCritLevel, 1);
                    }
                    else
                    {
                        crits = 1;
                    }
                    critLevel = Mathf.Min(oldCritLevel + crits, maxCrits);
                    critStat.SetValue(critLevel);
                    
                    Control.mod.Logger.LogDebug($"component crits={crits} maxCrits={maxCrits} oldCritLevel={oldCritLevel} critLevel={critLevel}");
                }

                var collectionStatisticName = criticalEffects.Linked?.CollectionStatisticName;
                if (!string.IsNullOrEmpty(collectionStatisticName))
                {
                    var statisticName = ScopedId(collectionStatisticName, mechComponent, criticalEffects.Scope);
                    
                    var collection = mech.StatCollection;
                    var critStat = collection.GetStatistic(statisticName) ?? collection.AddStatistic(statisticName, 0);
                    oldCritLevel = critStat.Value<int>();
                    var maxCrits = criticalEffects.PenalizedEffectIDs.Length;
                    critLevel = Mathf.Min(oldCritLevel + crits, criticalEffects.PenalizedEffectIDs.Length);
                    critStat.SetValue(critLevel);

                    Control.mod.Logger.LogDebug($"linked crits={crits} maxCrits={maxCrits} oldCritLevel={oldCritLevel} critLevel={critLevel}");
                }
            }

            {
                if (mechLocationDestroyed || critLevel >= criticalEffects.PenalizedEffectIDs.Length)
                {
                    damageLevel = ComponentDamageLevel.Destroyed;
                }
                else
                {
                    damageLevel = ComponentDamageLevel.Penalized;
                }

                SetDamageLevel(mechComponent, hitInfo, damageLevel);
            
                if (criticalEffects.Linked != null
                    && criticalEffects.Linked.SharedDamageLevel
                    && !string.IsNullOrEmpty(criticalEffects.Linked.CollectionStatisticName))
                {
                    var collectionStatisticName = criticalEffects.Linked?.CollectionStatisticName;
                    var scopedId = ScopedId(collectionStatisticName, mechComponent, criticalEffects.Scope);
                
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

                        var otherStatisticName = criticalEffects.Linked?.CollectionStatisticName;
                        if (string.IsNullOrEmpty(otherStatisticName))
                        {
                            continue;
                        }
                    
                        var otherScopedId = ScopedId(otherStatisticName, mc, ce.Scope);
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
                
                if (oldCritLevel > 0)
                {
                    effectIds.AddRange(criticalEffects.PenalizedEffectIDs[oldCritLevel]);
                }
                
                if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    effectIds.AddRange(criticalEffects.DestroyedDisableEffectIds);
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
                
                if (critLevel < criticalEffects.PenalizedEffectIDs.Length)
                {
                    effectIds = criticalEffects.PenalizedEffectIDs[critLevel];
                }
                
                if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    effectIds = criticalEffects.DestroyedEffectIds;
                }
                
                // collect disabled effects, probably easier to cache these in a mech statistic
                var disabledEffectIds = DisabledScopedIdsOnMech(mech);
                
                foreach (var effectId in effectIds)
                {
                    var scopedId = ScopedId(effectId, mechComponent, criticalEffects.Scope);
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
                    new Text("{0} " + (crits == 1 ? "CRIT" : "CRIT X" + crits), mechComponent.UIName),
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

                foreach (var effectId in ce.DestroyedDisableEffectIds)
                {
                    var scopedId = ScopedId(effectId, mc, ce.Scope);
                    disabledEffectIds.Add(scopedId);
                }
            }

            return disabledEffectIds;
        }
            
        private static string ScopedId(string id, MechComponent mechComponent, CriticalEffects.ScopeEnum scope)
        {
            switch (scope)
            {
                case CriticalEffects.ScopeEnum.Location:
                    var locationId = Mech.GetAbbreviatedChassisLocation(mechComponent.mechComponentRef.MountedLocation);
                    return $"{id}_{locationId}";
                case CriticalEffects.ScopeEnum.Component:
                    var uid = mechComponent.uid;
                    return $"{id}_{uid}";
                default:
                    return id;
            }
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
                var appliedEffectId = AppliedEffectId();
                
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
                
                actor.Combat.EffectManager.CreateEffect(
                    effectData, appliedEffectId, -1,
                    actor, actor,
                    default(WeaponHitInfo), 0, false);
    
                if (tracked)
                {
                    // make sure created effects are removed once component got destroyed
                    mechComponent.createdEffectIDs.Add(appliedEffectId);
                }
            }
    
            internal void CancelCriticalEffect()
            {
                var appliedEffectId = AppliedEffectId();
    
                var actor = mechComponent.parent;
                var statusEffects = actor.Combat.EffectManager
                    .GetAllEffectsWithID(appliedEffectId)
                    .Where(e => e.Target == actor);
    
                foreach (var statusEffect in statusEffects)
                {
                    actor.CancelEffect(statusEffect);
                    mechComponent.createdEffectIDs.Remove(appliedEffectId);
                }
            }
            
            private string AppliedEffectId()
            {
                var id = $"MECriticalHitEffect_{effectId}_{mechComponent.parent.GUID}";
                return ScopedId(id, mechComponent, scope);
            }
        }
    }
}