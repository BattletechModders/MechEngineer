
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CriticalHitStates")]
    public class CriticalHitStates : SimpleCustomComponent
    {
        public int MaxStates { get; set; }
        public DeathMethod DeathMethod { get; set; }
        public CriticalHitEffect[] HitEffects { get; set; }
    }

    public class CriticalHitEffect
    {
        public int State = 1;
        public EffectData StatusEffect = null;
    }
}
