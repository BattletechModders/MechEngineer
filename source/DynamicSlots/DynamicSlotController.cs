using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
using UIWidgetsSamples;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer
{
    public static class DynamicSlotController
    {
        private class loc_info
        {
            public ChassisLocations ChassisLocation { get { return location.Location; } }
            public int MaxSlots { get; private set; }
            public int UsedSlots { get { return used_slots.GetValue<int>(); } }


            private LocationLoadoutDef location;
            private List<Image> FillerImages;
            private MechLabLocationWidget loadout;
            private Traverse used_slots;

            public loc_info(MechLabLocationWidget widget, List<Image> images, LocationLoadoutDef location)
            {
                this.location = location;
                this.FillerImages = images;
                this.loadout = widget;

                var traverse = Traverse.Create(loadout);
                MaxSlots = traverse.Field("maxSlots").GetValue<int>();
                used_slots = traverse.Field("usedSlots");
            }

            public int Refresh(int slots, bool fit)
            {
                int used = UsedSlots;
                for (int i = 0; i < MaxSlots; i++)
                {
                    if (i < used)
                        FillerImages[i].gameObject.SetActive(false);
                    else if (i - used < slots)
                    {
                        FillerImages[i].gameObject.SetActive(true);
                        FillerImages[i].color = fit ? Control.settings.FitColor : Control.settings.UnFitColor;
                    }
                    else
                        FillerImages[i].gameObject.SetActive(false);
                }

                return slots - MaxSlots + used;
            }

            public void Clear()
            {
                foreach (var fillerImage in FillerImages)
                {
                    GameObject.Destroy(fillerImage);
                }
            }
        }


        private static Dictionary<ChassisLocations, loc_info> locations;
        private static MechLabPanel mech_lab;

        public static MechLabPanel MechLab
        {
            get => mech_lab;
            set
            {
                mech_lab = value;
                if (mech_lab == null)
                    foreach (var pair in locations)
                    {
                        pair.Value.Clear();
                    }
            }
        }

        internal static void RegisterLocation(MechLabLocationWidget instance, List<Image> images, LocationLoadoutDef loadout)
        {
            var location = new loc_info(instance, images, loadout);

            if (locations == null)
                locations = new Dictionary<ChassisLocations, loc_info>();

            //Control.mod.Logger.Log(string.Format("{0} Registrer, {1}/{2} slots", location.ChassisLocation, location.UsedSlots, location.MaxSlots));

            if (locations.TryGetValue(location.ChassisLocation, out var temp))
                locations[location.ChassisLocation] = location;
            else
                locations.Add(location.ChassisLocation, location);
        }

        internal static void RefreshData(MechDef def)
        {
            if (mech_lab == null)
                return;

            var slots = new MechDefSlots(def);
            var current = slots.Reserved;
            
            foreach (var pair in locations)
            {
                current = pair.Value.Refresh(current, slots.IsFit);
            }
        }

        public static void ValidateMech(Dictionary<MechValidationType, List<string>> errors,
            MechValidationLevel validationLevel, MechDef mechDef)
        {
            var slots = new MechDefSlots(mechDef);
            if (slots.IsOverloaded)
            {
                errors[MechValidationType.InvalidInventorySlots]
                    .Add($"RESERVED SLOTS: {slots.Reserved} criticals reserved, but have only {slots.Total - slots.Used}");
            }
            RefreshData(mechDef);
        }

        [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
        public static class OnMechLabDropPatch
        {
            [HarmonyPriority(Priority.High)]
            public static void Prefix(MechLabLocationWidget __instance, MechLabPanel ___mechLab, ref int ___usedSlots, int ___maxSlots)
            {
                var slots = new MechDefSlots(___mechLab.activeMechDef);
                var leftOver = Mathf.Max(slots.Total - (slots.Used + slots.Reserved), 0);
                var free = Mathf.Min(leftOver, ___maxSlots - ___usedSlots);

                ___usedSlots = ___maxSlots - free;
            }

            public static void Postfix(List<MechLabItemSlotElement> ___localInventory, MechLabPanel ___mechLab, ref int ___usedSlots)
            {
                ___usedSlots = MechDefSlots.GetUsedSlots(___localInventory.Select(s => s.ComponentRef));
                RefreshData(___mechLab.activeMechDef);
            }
        }

        internal static bool ValidateMechCanBeFielded(MechDef mechDef)
        {
            return new MechDefSlots(mechDef).IsOverloaded;
        }
    }

    internal class MechDefSlots
    {
        internal int Total { get; }
        internal int Used { get; }
        internal int Reserved { get; }

        internal MechDefSlots(MechDef mechDef)
        {
            Total = GetTotalSlots(mechDef.Chassis);
            Used = GetUsedSlots(mechDef.Inventory);
            Reserved = GetReservedSlots(mechDef.Inventory);
        }

        internal bool IsFit => !IsOverloaded;
        internal bool IsOverloaded => Used + Reserved > Total;

        private static int GetTotalSlots(ChassisDef chassisDef)
        {
            var total = chassisDef.LeftArm.InventorySlots;
            total += chassisDef.RightArm.InventorySlots;

            total += chassisDef.LeftLeg.InventorySlots;
            total += chassisDef.RightLeg.InventorySlots;

            total += chassisDef.RightTorso.InventorySlots;
            total += chassisDef.LeftTorso.InventorySlots;

            total += chassisDef.CenterTorso.InventorySlots;
            total += chassisDef.Head.InventorySlots;

            return total;
        }

        internal static int GetUsedSlots(IEnumerable<MechComponentRef> inventory)
        {
            return inventory.Sum(i => i.Def.InventorySize);
        }

        private static int GetReservedSlots(IEnumerable<MechComponentRef> inventory)
        {
            return inventory.Select(i => i.Def).OfType<IDynamicSlots>().Sum(i => i.ReservedSlots );
        }
    }
}