
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ChassisQuirks")]
    public class ChassisQuirks : SimpleCustom<ChassisDef>
    {
        public EffectData[] statusEffects { get; set; }
    }
}