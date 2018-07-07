
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CriticalHitStates")]
    public class CriticalHitStates : SimpleCustomComponent
    {
        public int CriticalHitStatesMaxCount { get; set; }
        public CriticalHitEffect[] CriticalHitEffects { get; set; }
    }

    public class CriticalHitEffect
    {
        public int CriticalHitState = 1;
        public EffectData StatusEffect = null;
    }
}
