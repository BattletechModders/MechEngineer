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

            void AddDescription(string prefix, string effectId)
            {
                var effectData = CriticalEffectsFeature.GetEffectData(effectId);
                if (effectData == null || effectData.targetingData.showInStatusPanel == false)
                {
                    return;
                }

                var description = CriticalEffectsFeature.settings.DescriptionUseName ? new Text(effectData.Description.Name).ToString() : new Text(effectData.Description.Details).ToString();
                    
                var text = $"{prefix}: {description}";
                    
                descriptions.Add(text);
            }

            var i = 0;
            foreach (var effectIDs in PenalizedEffectIDs)
            {
                i++;
                foreach (var id in effectIDs)
                {
                    AddDescription(new Text(CriticalEffectsFeature.settings.CritHitPrefix,i).ToString(),id);
                }
            }
            
            foreach (var id in OnDestroyedEffectIDs)
            {
                AddDescription(new Text(CriticalEffectsFeature.settings.CritDestroyedPrefix).ToString(), id);
            }

            if (DeathMethod != DeathMethod.NOT_SET)
            {
                descriptions.Add(new Text(CriticalEffectsFeature.settings.CritDestroyedPrefix).ToString() + new Text(DeathMethod.ToString()));
            }

            if (HasLinked)
            {
                descriptions.Add(new Text(CriticalEffectsFeature.settings.CritLinked).ToString()+"'"+ new Text(LinkedStatisticName)+"'");
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
