using BattleTech;
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
    public override UnitType GetActorType() {
      return UnitType.Turret;
    }
  }
}