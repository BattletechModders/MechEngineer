using System.Collections.Generic;
using BattleTech;
using CustomComponents;
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

                var description = Control.settings.CriticalEffectsDescriptionUseName ? effectData.Description.Name : effectData.Description.Details;
                    
                var text = $"{prefix}: {description}";
                    
                descriptions.Add(text);
            }

            var i = 0;
            foreach (var effectIDs in PenalizedEffectIDs)
            {
                i++;
                foreach (var id in effectIDs)
                {
                    AddDescription($"HIT {i}", id);
                }
            }
            
            foreach (var id in OnDestroyedEffectIDs)
            {
                AddDescription($"DESTROYED", id);
            }

            if (DeathMethod != DeathMethod.NOT_SET)
            {
                descriptions.Add($"DESTROYED: Mech is incapacitated, reason is {DeathMethod}");
            }

            if (HasLinked)
            {
                descriptions.Add($"Critical hits are linked to '{LinkedStatisticName}'");
            }

            
            BonusDescriptions.AddBonusDescriptions(
                Def.Description,
                descriptions,
                Control.settings.BonusDescriptionsElementTemplate,
                Control.settings.CriticalEffectsDescriptionTemplate
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
