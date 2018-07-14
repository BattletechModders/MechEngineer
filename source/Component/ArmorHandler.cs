using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class ArmorHandler : IDescription, IProcessWeaponHit
    {
        internal static ArmorHandler Shared = new ArmorHandler();

        public string CategoryName { get; } = "Armor";

        public bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsCategory(CategoryName))
            {
                return false;
            }

            return true;
        }
    }
}