using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects
{
    [CustomComponent("VehicleCriticalEffects")]
    public class VehicleCriticalEffects : CriticalEffects
    {
        public override string GetActorTypeDescription()
        {
            return "Vehicle";
        }
    public override UnitType GetActorType() {
      return UnitType.Vehicle;
    }
  }
}