using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.OmniSlots
{
    internal class HardpointOmniUsageCalculator
    {
        internal HardpointStat Ballistic => stats[WeaponCategoryEnumeration.GetBallistic()];
        internal HardpointStat Energy => stats[WeaponCategoryEnumeration.GetEnergy()];
        internal HardpointStat Missile => stats[WeaponCategoryEnumeration.GetMissile()];
        internal HardpointStat Small => stats[WeaponCategoryEnumeration.GetSupport()];

        internal static WeaponCategoryValue MapToBasicType(WeaponCategoryValue value)
        {
            if (value.IsBallistic)
            {
                return WeaponCategoryEnumeration.GetBallistic();
            }
            else if (value.IsEnergy)
            {
                return WeaponCategoryEnumeration.GetEnergy();
            }
            else if (value.IsMissile)
            {
                return WeaponCategoryEnumeration.GetMissile();
            }
            else if (value.IsSupport)
            {
                return WeaponCategoryEnumeration.GetSupport();
            }
            else
            {
                Control.Logger.Warning.Log($"Unsupported weapon category for {value.ID}");
                return WeaponCategoryEnumeration.GetNotSetValue();
            }
        }

        private readonly Dictionary<WeaponCategoryValue, HardpointStat> stats = new Dictionary<WeaponCategoryValue, HardpointStat>();

        public override string ToString()
        {
            return $"B{Ballistic.VanillaUsage}/{Ballistic.VanillaMax} E{Energy.VanillaUsage}/{Energy.VanillaMax} M{Missile.VanillaUsage}/{Missile.VanillaMax} S{Small.VanillaUsage}/{Small.VanillaMax} O{OmniUsage}/{OmniMax}";
        }

        internal HardpointOmniUsageCalculator(IEnumerable<MechComponentDef> items, HardpointDef[] hardpoints)
        {
            stats[WeaponCategoryEnumeration.GetBallistic()] = new HardpointStat(WeaponCategoryEnumeration.GetBallistic());
            stats[WeaponCategoryEnumeration.GetEnergy()] = new HardpointStat(WeaponCategoryEnumeration.GetEnergy());
            stats[WeaponCategoryEnumeration.GetMissile()] = new HardpointStat(WeaponCategoryEnumeration.GetMissile());
            stats[WeaponCategoryEnumeration.GetSupport()] = new HardpointStat(WeaponCategoryEnumeration.GetSupport());

            SetUsage(items);
            SetMax(hardpoints);
        }

        private void SetUsage(IEnumerable<MechComponentDef> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item.ComponentType == ComponentType.Weapon && item is WeaponDef weaponDef && stats.TryGetValue(MapToBasicType(weaponDef.WeaponCategoryValue), out var stat))
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

                if (stats.TryGetValue(MapToBasicType(hardpoint.WeaponMountValue), out var stat))
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

        internal bool CanAdd(MechComponentDef newComponentDef)
        {
            if (newComponentDef.ComponentType != ComponentType.Weapon)
            {
                return true;
            }

            var weapon = newComponentDef as WeaponDef;
            if (stats.TryGetValue(MapToBasicType(weapon.WeaponCategoryValue), out var stat))
            {
                return stat.TheoreticalMax > 0;
            }
            else
            {
                return true;
            }
        }
    }
}