
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CriticalEffects")]
    public class CriticalEffects : SimpleCustomComponent
    {
        public string[][] PenalizedEffectIDs { get; set; } = new string[0][];
        public string[] OnDestroyedEffectIDs { get; set; } = new string[0];
        public string[] OnDestroyedDisableEffectIds { get; set; } = new string[0];

        public DeathMethod DeathMethod { get; set; } = DeathMethod.NOT_SET;
        
        public ScopeEnum Scope { get; set; } = ScopeEnum.Component;
        public enum ScopeEnum
        {
            Component, Mech
        }
        
        public string LinkedStatisticName = null;

        public bool HasLinked => !string.IsNullOrEmpty(LinkedStatisticName);
    }
}
