using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class HardpointCounter
    {
        internal readonly int numBallistic;
        internal readonly int numEnergy;
        internal readonly int numMissile;
        internal readonly int numSmall;

        private static bool isBH(string prefabName)
        {
            return prefabName.Contains("_bh") || prefabName.Contains("_ac");
        }

        private static bool isMH(string prefabName)
        {
            return prefabName.Contains("_mh") || prefabName.Contains("_lrm10");
        }

        private static bool isEH(string prefabName)
        {
            return prefabName.Contains("_eh") || prefabName.Contains("_laser");
        }

        private static bool isAH(string prefabName)
        {
            return prefabName.Contains("_ah")
                   || prefabName.Contains("_laser_eh")
                   || prefabName.Contains("_flamer_eh")
                   || prefabName.Contains("_machinegun_bh")
                   || prefabName.Contains("_mg_bh");
        }

        internal HardpointCounter(string[][] weaponHardpoints)
        {
            foreach (var hardpoint in weaponHardpoints)
            {
                bool bh = false, mh = false, eh = false, ah = false;
                foreach (var prefab in hardpoint)
                {
                    if (isBH(prefab))
                    {
                        bh = true;
                    }

                    if (isMH(prefab))
                    {
                        mh = true;
                    }

                    if (isEH(prefab))
                    {
                        eh = true;
                    }

                    if (isAH(prefab))
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
                list.AddRange(CreateEntries(WeaponCategoryEnumeration.GetBallistic(), numBallistic));
                list.AddRange(CreateEntries(WeaponCategoryEnumeration.GetEnergy(), numEnergy));
                list.AddRange(CreateEntries(WeaponCategoryEnumeration.GetMissile(), numMissile));
                list.AddRange(CreateEntries(WeaponCategoryEnumeration.GetSupport(), numSmall));
                return list.ToArray();
            }
        }

        private IEnumerable<HardpointDef> CreateEntries(WeaponCategoryValue category, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new HardpointDef(category, false);
            }
        }
    }
}