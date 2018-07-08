using System.Collections.Generic;
using BattleTech;

namespace MechEngineer
{
    internal class HardpointCounter
    {
        internal readonly int numBallistic;
        internal readonly int numEnergy;
        internal readonly int numMissile;
        internal readonly int numSmall;

        internal HardpointCounter(ChassisDef chassisDef, ChassisLocations location)
        {
            MechStatisticsRules.GetHardpointCountForLocation(chassisDef, location, ref numBallistic, ref numEnergy, ref numMissile, ref numSmall);
        }

        internal HardpointCounter(string[][] weaponHardpoints)
        {
            foreach (var hardpoint in weaponHardpoints)
            {
                bool bh = false, mh = false, eh = false, ah = false;
                foreach (var prefab in hardpoint)
                {
                    if (prefab.Contains("_bh") || prefab.Contains("_ac"))
                    {
                        bh = true;
                    }

                    if (prefab.Contains("_mh") || prefab.Contains("_lrm10"))
                    {
                        mh = true;
                    }

                    if (prefab.Contains("_eh") || prefab.Contains("_laser"))
                    {
                        eh = true;
                    }

                    if (prefab.Contains("_ah") || prefab.Contains("_laser_eh") || prefab.Contains("_flamer_eh") || prefab.Contains("_machinegun_bh") || prefab.Contains("_mg_bh"))
                    {
                        ah = true;
                    }
                }

                if (bh)
                {
                    numBallistic++;
                }

                if (mh)
                {
                    numMissile++;
                }

                if (eh)
                {
                    numEnergy++;
                }

                if (ah)
                {
                    numSmall++;
                }
            }
        }

        internal HardpointDef[] HardpointsDefs
        {
            get
            {
                var list = new List<HardpointDef>();
                list.AddRange(CreateEntries(WeaponCategory.Ballistic, numBallistic));
                list.AddRange(CreateEntries(WeaponCategory.Energy, numEnergy));
                list.AddRange(CreateEntries(WeaponCategory.Missile, numMissile));
                list.AddRange(CreateEntries(WeaponCategory.AntiPersonnel, numSmall));
                return list.ToArray();
            }
        }

        private IEnumerable<HardpointDef> CreateEntries(WeaponCategory category, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new HardpointDef(category, false);
            }
        }
    }
}