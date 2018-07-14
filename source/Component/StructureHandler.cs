using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class StructureHandler : IDescription,  IProcessWeaponHit
    {
        internal static StructureHandler Shared = new StructureHandler();

        public string CategoryName { get; } = "Structure";


        public bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsCategory("Structure"))
            {
                return false;
            }

            return true;
        }
    }
}