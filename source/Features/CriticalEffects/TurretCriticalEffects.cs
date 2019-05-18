using CustomComponents;

namespace MechEngineer.Features.CriticalEffects
{
    [CustomComponent("TurretCriticalEffects")]
    public class TurretCriticalEffects : CriticalEffects
    {
        public override string GetActorTypeDescription()
        {
            return "Turret";
        }
    }
}