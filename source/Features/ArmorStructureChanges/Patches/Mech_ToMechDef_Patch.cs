using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    // fixing factor not being considerd for campaign stuff, this removes the factor again before switching back to campaign
    [HarmonyPatch(typeof(Mech), "ToMechDef")]
    public static class Mech_ToMechDef_Patch
    {
        public static void Postfix(Mech __instance, MechDef __result)
        {
            var mech = __instance;
            var mechDef = __result;
            var armorFactor = Mech_get_ArmorMultiplier_Patch.GetFactorForMech(mech);
            var structureFactor = Mech_get_StructureMultiplier_Patch.GetFactorForMech(mech);

            var adapter = new MechDefAdapter(mechDef);
            foreach (var mechLocationDef in adapter.Locations)
            {
                if (mechLocationDef.CurrentInternalStructure > 0)
                {
                    mechLocationDef.CurrentInternalStructure /= structureFactor;
                    var chassisLocationDef = mechDef.Chassis.GetLocationDef(mechLocationDef.Location);
                    if (Mathf.Approximately(mechLocationDef.CurrentInternalStructure, chassisLocationDef.InternalStructure))
                    {
                        mechLocationDef.CurrentInternalStructure = chassisLocationDef.InternalStructure;
                    }
                }

                if (mechLocationDef.CurrentArmor > 0)
                {
                    mechLocationDef.CurrentArmor /= armorFactor;
                    if (Mathf.Approximately(mechLocationDef.CurrentArmor, mechLocationDef.AssignedArmor))
                    {
                        mechLocationDef.CurrentArmor = mechLocationDef.AssignedArmor;
                    }
                }

                if (mechLocationDef.CurrentRearArmor > 0)
                {
                    mechLocationDef.CurrentRearArmor /= armorFactor;
                    if (Mathf.Approximately(mechLocationDef.CurrentRearArmor, mechLocationDef.AssignedRearArmor))
                    {
                        mechLocationDef.CurrentRearArmor = mechLocationDef.AssignedRearArmor;
                    }
                }
            }
        }
    }
}
