﻿using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects
{
    [CustomComponent("MechCriticalEffects")]
    public class MechCriticalEffects : CriticalEffects
    {
        public override string GetActorTypeDescription()
        {
            return "Mech";
        }
    public override UnitType GetActorType() {
      return UnitType.Mech;
    }
  }
}