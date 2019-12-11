using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Localize;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.CriticalEffects
{
    [CustomComponent("CriticalEffects")]
    public class CriticalEffects : SimpleCustomComponent, IAfterLoad, IIsDestroyed
    {
        public string[][] PenalizedEffectIDs { get; set; } = new string[0][];
        public string[] OnDestroyedEffectIDs { get; set; } = new string[0];
        public string[] OnDestroyedDisableEffectIds { get; set; } = new string[0];

        public DeathMethod DeathMethod { get; set; } = DeathMethod.NOT_SET;
        public string OnDestroyedVFXName { get; set; } = null;
        public string OnDestroyedAudioEventName { get; set; } = null;

        public string LinkedStatisticName = null;

        public bool HasLinked => !string.IsNullOrEmpty(LinkedStatisticName);
        
        public string CritFloatieMessage = null;
        public string DestroyedFloatieMessage = null;

        public virtual string GetActorTypeDescription()
        {
            return null;
        }
        
        public void OnLoaded(Dictionary<string, object> values)
        {
            var descriptions = new List<string>();

            string GetEffectDescription(string effectId)
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

            if (HasLinked)
            {
                descriptions.Add(new Text(CriticalEffectsFeature.settings.CritLinkedText, LinkedStatisticName).ToString());
            }
            
            var descriptionTemplate = CriticalEffectsFeature.settings.DescriptionTemplate;
            {
                var actorDescription = GetActorTypeDescription();
                if (actorDescription != null)
                {
                    descriptionTemplate = $"{actorDescription} {descriptionTemplate}";
                }
            }
            
            BonusDescriptions.AddBonusDescriptions(
                Def.Description,
                descriptions,
                CriticalEffectsFeature.settings.ElementTemplate,
                descriptionTemplate
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

}
