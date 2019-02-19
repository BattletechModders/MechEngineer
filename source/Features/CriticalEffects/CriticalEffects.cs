
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CriticalEffects")]
    public class CriticalEffects : SimpleCustomComponent
    {
        public string[][] PenalizedEffectIDs { get; set; } = new string[0][];

        public DeathMethod DeathMethod { get; set; } = DeathMethod.NOT_SET;
        public string[] DestroyedEffectIds { get; set; } = new string[0];
        public string[] DestroyedDisableEffectIds { get; set; } = new string[0];
        
        public ScopeEnum Scope { get; set; } = ScopeEnum.Component;
        public enum ScopeEnum
        {
            Component, Location, Mech
        }
        
        public LinkedClass Linked = null;
        public class LinkedClass
        {
            public string CollectionStatisticName { get; set; } = null;
            public bool SharedDamageLevel { get; set; } = false;
        }
    }
}
