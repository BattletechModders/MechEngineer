
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("CASE")]
    public class CASEComponent : SimpleCustomComponent
    {
        public float? MaximumDamage { get; set; } = null;
        public bool AllLocations { get; set; } = false;
    }
}