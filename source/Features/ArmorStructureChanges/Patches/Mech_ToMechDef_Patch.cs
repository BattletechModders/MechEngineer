﻿using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.sorting;
using UnityEngine;

namespace MechEngineer.Features.ArmorStructureChanges.Patches
{
    // fixing factor not being considerd for campaign stuff, this removes the factor again before switching back to campaign
    [HarmonyPatch(typeof(Mech), nameof(Mech.ToMechDef))]
    public static class Mech_ToMechDef_Patch
    {
        public static void Postfix(Mech __instance, MechDef __result)
        {
            try
            {
                UndoArmorStructureChanges(__instance, __result);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        private static void UndoArmorStructureChanges(Mech mech, MechDef mechDef)
        {
            var armorFactor = ArmorStructureChangesFeature.GetArmorFactorForMech(mech);
            var structureFactor = ArmorStructureChangesFeature.GetStructureFactorForMech(mech);

            var adapter = new MechDefAdapter(mechDef);
            foreach (var mechLocationDef in adapter.Locations)
            {
                var chassisLocationDef = mechDef.Chassis.GetLocationDef(mechLocationDef.Location);

                mechLocationDef.CurrentInternalStructure = ReverseFactor(structureFactor, mechLocationDef.CurrentInternalStructure, chassisLocationDef.InternalStructure);
                mechLocationDef.CurrentArmor = ReverseFactor(armorFactor, mechLocationDef.CurrentArmor, mechLocationDef.AssignedArmor);
                mechLocationDef.CurrentRearArmor = ReverseFactor(armorFactor, mechLocationDef.CurrentRearArmor, mechLocationDef.AssignedRearArmor);
            }
        }

        private static float ReverseFactor(float factor, float current, float max)
        {
            if (current <= 0)
            {
                return current;
            }

            current = Mathf.Min(current / factor, max);
            if (Mathf.Approximately(current, max))
            {
                current = max;
            }
            else if (Mathf.Approximately(current, 0))
            {
                current = 0;
            }

            return current;
        }
    }
}
