
using BattleTech;

namespace MechEngineer
{
    interface ICriticalHitStates
    {
        int CriticalHitStatesMaxCount { get; }
        CriticalHitEffect[] CriticalHitEffects { get; }
    }

    public class CriticalHitEffect
    {
        public int CriticalHitState = 1;
        public EffectData StatusEffect = null;
    }
}
