
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ComponentExplosion")]
    public class ComponentExplosion : SimpleCustomComponent
    {
        public float ExplosionDamage { get; set; }
        public float ExplosionDamagePerAmmo { get; set; }
    }
}