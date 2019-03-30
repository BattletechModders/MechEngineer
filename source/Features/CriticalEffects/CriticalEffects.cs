
using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CriticalEffects")]
    public class CriticalEffects : SimpleCustomComponent, IAfterLoad //, ICheckIsDead
    {
        public string[][] PenalizedEffectIDs { get; set; } = new string[0][];
        public string[] OnDestroyedEffectIDs { get; set; } = new string[0];
        public string[] OnDestroyedDisableEffectIds { get; set; } = new string[0];

        public DeathMethod DeathMethod { get; set; } = DeathMethod.NOT_SET;
        
        public string LinkedStatisticName = null;

        public bool HasLinked => !string.IsNullOrEmpty(LinkedStatisticName);
        
        public string CritFloatieMessage = null;
        public string DestroyedFloatieMessage = null;
        
        public void OnLoaded(Dictionary<string, object> values)
        {
            var descriptions = new List<string>();
            
            var i = 0;
            foreach (var effectIDs in PenalizedEffectIDs)
            {
                i++;
                foreach (var id in effectIDs)
                {
                    var effectData = CriticalEffectsHandler.GetEffectData(id);
                    if (effectData == null || effectData.targetingData.showInStatusPanel == false)
                    {
                        continue;
                    }
                    
                    var text = $"HIT {i}: {effectData.Description.Details}";
                    
                    descriptions.Add(text);
                }
            }
            
            foreach (var id in OnDestroyedEffectIDs)
            {
                var effectData = CriticalEffectsHandler.GetEffectData(id);
                if (effectData == null || effectData.targetingData.showInStatusPanel == false)
                {
                    continue;
                }
                
                var text = $"DESTROYED: {effectData.Description.Details}";
                    
                descriptions.Add(text);
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

        // TODO should not be necessary as actor IsDead should already return true
        //public bool IsActorDestroyed(MechComponent component, AbstractActor actor)
        //{
        //    if (DeathMethod == DeathMethod.NOT_SET)
        //    {
        //        return false;
        //    }

        //    return component.DamageLevel == ComponentDamageLevel.Destroyed;
        //}
    }
}
