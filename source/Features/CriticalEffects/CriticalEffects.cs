using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using FluffyUnderware.DevTools.Extensions;
using MechEngineer.Features.PlaceholderEffects;
using UnityEngine;

namespace MechEngineer.Features.CriticalEffects;

internal readonly struct CriticalEffects
{
    private readonly ComponentCriticals _criticals;
    private MechComponent component => _criticals.component;
    private AbstractActor actor => component.parent;

    internal CriticalEffects(MechComponent component)
    {
        _criticals = new(component);
        Effects = FetchCriticalEffects();
        if (Effects != null)
        {
            Log.Main.Trace?.Log($"Found matching {Effects.GetType()}");
        }
    }

    private bool IsLocationDestroyed => actor.StructureForLocation(component.Location) <= 0f;

    internal readonly CriticalEffectsCustom? Effects;
    private bool HasLinked => Effects?.LinkedStatisticName != null;
    private bool IsArmored => Effects?.IsArmored ?? false;
    private CriticalEffectsCustom? FetchCriticalEffects()
    {
        var customs = component.componentDef.GetComponents<CriticalEffectsCustom>().ToList();
        Log.Main.Trace?.Log($"Finding CriticalEffects custom for {component.componentDef.Description.Id} on Actor Id={actor.Description.Id} type={actor.GetType()}");

        if (actor is Mech mech)
        {
            {
                var unitTypes = UnitTypeDatabase.Instance.GetUnitTypes(mech.MechDef);
                if (unitTypes is { Count: > 0 })
                {
                    Log.Main.Trace?.Log($"Found mech.UnitTypes=[{string.Join(",", unitTypes)}] on ChassisID={mech.MechDef.ChassisID}");
                    foreach (var custom in customs.OfType<MechCriticalEffectsCustom>())
                    {
                        Log.Main.Trace?.Log($"Found custom.UnitTypes=[{string.Join(",", custom.UnitTypes)}]");
                        if (custom.UnitTypes.All(t => unitTypes.Contains(t)))
                        {
                            return custom;
                        }
                    }
                }
            }

            {
                var custom = customs.OfType<MechCriticalEffectsCustom>().FirstOrDefault(x => x.UnitTypes.Length == 0);
                if (custom != null)
                {
                    return custom;
                }
            }
        }

        if (actor is Turret)
        {
            var custom = customs.FirstOrDefault(x => x is TurretCriticalEffectsCustom);
            if (custom != null)
            {
                return custom;
            }
        }

        if (actor is Vehicle)
        {
            var custom = customs.FirstOrDefault(x => x is VehicleCriticalEffectsCustom);
            if (custom != null)
            {
                return custom;
            }
        }

        {
            var custom = customs.FirstOrDefault(x => x is not MechCriticalEffectsCustom && x is not TurretCriticalEffectsCustom && x is not VehicleCriticalEffectsCustom);
            return custom;
        }
    }

    internal void Hit(WeaponHitInfo hitInfo, out ComponentDamageLevel damageLevel)
    {
        SetHits(hitInfo, out damageLevel);

        if (damageLevel == ComponentDamageLevel.Destroyed)
        {
            PostDestructionEvents(hitInfo);
        }
    }

    private void SetHits(WeaponHitInfo hitInfo, out ComponentDamageLevel damageLevel)
    {
        int effectsMax, effectsPrev, effectsNext;
        {
            var compCritsMax = _criticals.ComponentHitMax();
            var compCritsPrev = _criticals.ComponentHitCount();

            var possibleAddedHits = 1;
            if (IsLocationDestroyed)
            {
                possibleAddedHits = compCritsMax;
            }
            else if (IsArmored)
            {
                var compCritsArmoredPrev = _criticals.ComponentHitArmoredCount();
                if (compCritsArmoredPrev < compCritsMax)
                {
                    var randomCache = actor.Combat.AttackDirector.GetRandomFromCache(hitInfo, 1);
                    var isArmoredHit = compCritsArmoredPrev <= Mathf.RoundToInt(compCritsMax * randomCache[0]);
                    if (isArmoredHit)
                    {
                        var compCritsArmoredNext = compCritsArmoredPrev + 1;
                        _criticals.ComponentHitArmoredCount(compCritsArmoredNext);
                        damageLevel = component.DamageLevel;
                        return;
                    }
                }
            }

            var compCritsNext = Mathf.Min(compCritsMax, compCritsPrev + possibleAddedHits);
            var compCritsAdded = Mathf.Max(compCritsNext - compCritsPrev, 0);

            _criticals.ComponentHitCount(compCritsNext);

            // if max is reached, component is destroyed and no new effects can be applied
            // Destroyed components can still soak up crits, requires properly configured AIM from CAC
            effectsMax = Effects?.PenalizedEffectIDs.Length + 1 ?? DefaultEffectsMax();

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

        Log.Main.Debug?.Log(
            $"Component hit (uid={component.uid} Id={component.Description.Id} Location={component.Location}) " +
            $"effectsMax={effectsMax} effectsPrev={effectsPrev} effectsNext={effectsNext} " +
            $"damageLevel={damageLevel} HasEffects={Effects != null} LinkedStatisticName={Effects?.LinkedStatisticName}"
        );
    }

    private int DefaultEffectsMax()
    {
        if (CriticalEffectsFeature.settings.DefaultMaxCritsComponentTypes.Contains(component.componentType))
        {
            var slots = CriticalEffectsFeature.settings.DefaultMaxCritsPerSlots * component.componentDef.InventorySize;
            return Mathf.FloorToInt(slots) + 1; // last effect = Destroyed
        }
        return 1;
    }

    private int GroupHitCount(int? setHits = null)
    {
        if (actor == null)
        {
            throw new NullReferenceException();
        }

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
        if (Effects == null)
        {
            throw new NullReferenceException();
        }

        return ScopedId(Effects.LinkedStatisticName!);
    }

    private string ScopedId(string id)
    {
        if (PlaceholderInterpolation.Create(id, component, out var interpolation))
        {
            return interpolation.InterpolateEffectId(id);
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
        LocalSetDamageLevel(component, hitInfo, damageLevel);

        if (HasLinked)
        {
            var id = LinkedScopedId();

            Log.Main.Debug?.Log($"HasLinked id={id}");
            foreach (var otherMechComponent in actor.allComponents)
            {
                if (otherMechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
                {
                    continue;
                }

                var otherCriticals = otherMechComponent.CriticalEffects();
                if (!otherCriticals.HasLinked)
                {
                    continue;
                }

                var otherId = otherCriticals.LinkedScopedId();
                if (id == otherId)
                {
                    LocalSetDamageLevel(otherMechComponent, hitInfo, damageLevel);
                }

            }
        }

        static void LocalSetDamageLevel(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel)
        {
            Log.Main.Debug?.Log($"damageLevel={damageLevel} uid={mechComponent.uid} (Id={mechComponent.Description.Id} Location={mechComponent.Location})");
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
        if (Effects == null)
        {
            throw new NullReferenceException();
        }

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
        if (Effects == null)
        {
            throw new NullReferenceException();
        }
        if (actor == null)
        {
            throw new NullReferenceException();
        }

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
        Log.Main.Debug?.Log($"disabledEffectIds={string.Join(",", disabledScopedEffectIds.ToArray())}");
        foreach (var effectId in effectIds)
        {
            var simpleScopedEffectId = ScopedId(effectId);
            if (disabledScopedEffectIds.Contains(simpleScopedEffectId))
            {
                continue;
            }

            var resolvedEffectId = ScopedId(effectId);
            var util = new EffectIdUtil(effectId, resolvedEffectId, component);
            util.CreateCriticalEffect();
        }

        static HashSet<string> DisabledSimpleScopedEffectIdsOnActor(AbstractActor actor)
        {
            var iter = from mc in actor.allComponents
                where !mc.IsFunctional
                let ce = mc.CriticalEffects().Effects
                where ce != null
                from effectId in ce.OnDestroyedDisableEffectIds
                select mc.
                    CriticalEffects().ScopedId(effectId);

            return new(iter);
        }
    }

    private void PostDestructionEvents(WeaponHitInfo hitInfo)
    {
        if (Effects == null)
        {
            return;
        }
        if (actor == null)
        {
            throw new NullReferenceException();
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