using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.OmniSlots
{
    internal class HardpointOmniUsageCalculator
    {
        internal HardpointStat Ballistic => stats[WeaponCategory.Ballistic];
        internal HardpointStat Energy => stats[WeaponCategory.Energy];
        internal HardpointStat Missile => stats[WeaponCategory.Missile];
        internal HardpointStat Small => stats[WeaponCategory.AntiPersonnel];

        private readonly Dictionary<WeaponCategory, HardpointStat> stats = new Dictionary<WeaponCategory, HardpointStat>();

        public override string ToString()
        {
            return $"B{Ballistic.VanillaUsage}/{Ballistic.VanillaMax} E{Energy.VanillaUsage}/{Energy.VanillaMax} M{Missile.VanillaUsage}/{Missile.VanillaMax} S{Small.VanillaUsage}/{Small.VanillaMax} O{OmniUsage}/{OmniMax}";
        }

        internal HardpointOmniUsageCalculator(IEnumerable<MechComponentDef> items, HardpointDef[] hardpoints)
        {
            stats[WeaponCategory.Ballistic] = new HardpointStat(WeaponCategory.Ballistic);
            stats[WeaponCategory.Energy] = new HardpointStat(WeaponCategory.Energy);
            stats[WeaponCategory.Missile] = new HardpointStat(WeaponCategory.Missile);
            stats[WeaponCategory.AntiPersonnel] = new HardpointStat(WeaponCategory.AntiPersonnel);

            SetUsage(items);
            SetMax(hardpoints);
        }

        private void SetUsage(IEnumerable<MechComponentDef> items)
        {
            foreach (var item in items)
            {
                if (item.ComponentType == ComponentType.Weapon && item is WeaponDef weaponDef && stats.TryGetValue(weaponDef.Category, out var stat))
                {
                    stat.VanillaUsage++;
                }
            }
        }

        internal int OmniMax;
        internal int OmniUsage;
        internal int OmniFree => OmniMax - OmniUsage;

        private void SetMax(HardpointDef[] hardpoints)
        {
            foreach (var hardpoint in hardpoints)
            {
                if (hardpoint.Omni)
                {
                    OmniMax++;
                    continue;
                }

                if (stats.TryGetValue(hardpoint.WeaponMount, out var stat))
                {
                    stat.VanillaMax++;
                }
            }

            foreach (var stat in stats.Values)
            {
                OmniUsage += stat.VanillaMaxOver;
            }

            var freeOmni = OmniFree;
            foreach (var stat in stats.Values)
            {
                stat.OmniMax = OmniMax;
                stat.OmniFree = freeOmni;
            }
        }

        internal bool CanAdd(bool dropCheck, MechComponentDef newComponentDef)
        {
            if (newComponentDef.ComponentType != ComponentType.Weapon)
            {
                return true;
            }

            var weapon = newComponentDef as WeaponDef;
            if (stats.TryGetValue(weapon.Category, out var stat))
            {
                return dropCheck ? stat.DynamicFree > 0 : stat.TheoreticalMax > 0;
            }
            else
            {
                return true;
            }
        }
    }
}