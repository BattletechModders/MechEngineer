using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using CustomComponents.ExtendedDetails;
using Localize;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Misc;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("CriticalEffects")]
public class CriticalEffectsCustom : SimpleCustomComponent, IAfterLoad, IIsDestroyed
{
    public string[][] PenalizedEffectIDs { get; set; } = Array.Empty<string[]>();
    public string[] OnDestroyedEffectIDs { get; set; } = Array.Empty<string>();
    public string[] OnDestroyedDisableEffectIds { get; set; } = Array.Empty<string>();

    public DeathMethod DeathMethod { get; set; } = DeathMethod.NOT_SET;
    public string? OnDestroyedVFXName { get; set; } = null;
    public string? OnDestroyedAudioEventName { get; set; } = null;

    public string? LinkedStatisticName { get; set; } = null;

    public string? CritFloatieMessage { get; set; } = null;
    public string? DestroyedFloatieMessage { get; set; } = null;

    protected virtual UnitType GetUnitType()
    {
        return UnitType.UNDEFINED;
    }

    // how many crits can be absorbed incl. destruction of component itself
    [UsedBy(User.FieldRepairs)]
    public int MaxHits => PenalizedEffectIDs.Length + 1;

    public void OnLoaded(Dictionary<string, object> values)
    {
        var descriptions = new List<string>();

        string? GetEffectDescription(string effectId)
        {
            var effectData = CriticalEffectsFeature.GetEffectData(effectId);
            if (effectData == null || effectData.targetingData.showInStatusPanel == false)
            {
                return null;
            }
            return CriticalEffectsFeature.settings.DescriptionUseName ? effectData.Description.Name : effectData.Description.Details;
        }

        var i = 0;
        foreach (var effectIDs in PenalizedEffectIDs)
        {
            i++;
            foreach (var id in effectIDs)
            {
                var effectDesc = GetEffectDescription(id);
                if (effectDesc == null)
                {
                    continue;
                }
                descriptions.Add(new Text(CriticalEffectsFeature.settings.CritHitText, i, effectDesc).ToString());
            }
        }

        foreach (var id in OnDestroyedEffectIDs)
        {
            var effectDesc = GetEffectDescription(id);
            if (effectDesc == null)
            {
                continue;
            }
            descriptions.Add(new Text(CriticalEffectsFeature.settings.CritDestroyedText, effectDesc).ToString());
        }

        if (DeathMethod != DeathMethod.NOT_SET)
        {
            descriptions.Add(new Text(CriticalEffectsFeature.settings.CritDestroyedDeathText, DeathMethod).ToString());
        }

        if (LinkedStatisticName != null)
        {
            descriptions.Add(new Text(CriticalEffectsFeature.settings.CritLinkedText, LinkedStatisticName).ToString());
        }

        var descriptionTemplate = CriticalEffectsFeature.settings.DescriptionTemplate;
        {
            var actorType = GetUnitType();
            if (actorType != UnitType.UNDEFINED)
            {
                var actorDescription = actorType.ToString();
                descriptionTemplate = $"{actorDescription} {descriptionTemplate}";
            }
        }

        if (descriptions.Count == 0)
        {
            return;
        }

        BonusDescriptions.AddTemplatedExtendedDetail(
            ExtendedDetails.GetOrCreate(Def),
            descriptions,
            CriticalEffectsFeature.settings.ElementTemplate,
            descriptionTemplate,
            CriticalEffectsFeature.settings.DescriptionIdentifier,
            GetUnitType()
        );
    }

    public bool IsMechDestroyed(MechComponentRef component, MechDef mech)
    {
        if (DeathMethod == DeathMethod.NOT_SET)
        {
            return false;
        }

        return component.DamageLevel == ComponentDamageLevel.Destroyed;
    }
}
