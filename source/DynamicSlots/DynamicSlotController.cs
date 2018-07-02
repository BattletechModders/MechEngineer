using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;
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

        public static int GetReservedSlots(this MechDef mechDef)
        {
            return mechDef.Inventory.Select(i => i.Def).OfType<IDynamicSlots>().DefaultIfEmpty().Sum(i => i?.ReserverdSlots ?? 0);
        }

        public  static int GetTotalSlots(this MechDef mechDef)
        {
            var total = mechDef.Chassis.LeftArm.InventorySlots;
            total += mechDef.Chassis.RightArm.InventorySlots;

            total += mechDef.Chassis.LeftLeg.InventorySlots;
            total += mechDef.Chassis.RightLeg.InventorySlots;

            total += mechDef.Chassis.RightTorso.InventorySlots;
            total += mechDef.Chassis.LeftTorso.InventorySlots;

            total += mechDef.Chassis.CenterTorso.InventorySlots;
            total += mechDef.Chassis.Head.InventorySlots;

            return total;
        }

        public static int GetUsedSlots(this MechDef mechDef)
        {
            return mechDef.Inventory.Length == 0 ? 0 : mechDef.Inventory.Sum(i => i.Def.InventorySize);
        }

        public static void RegisterLocation(MechLabLocationWidget instance, List<Image> images, LocationLoadoutDef loadout)
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

        public static void RefreshData(MechDef def)
        {
            if (mech_lab == null)
                return;

            //Control.mod.Logger.Log("refresh required");

            var total = def.GetTotalSlots();
            var used = def.GetUsedSlots();
            var need = def.GetReservedSlots();
            var slots = need;

            //Control.mod.Logger.Log(string.Format("Refresh slots total:{0} used:{1} free:{2} need:{3}", total, used, total - used, need));

            foreach (var pair in locations)
            {
                slots = pair.Value.Refresh(slots, need <= total - used);
            }
        }

        public static void ValidateMech(Dictionary<MechValidationType, List<string>> errors,
            MechValidationLevel validationLevel, MechDef mechDef)
        {
            var total = mechDef.GetTotalSlots();
            var used = mechDef.GetUsedSlots();
            var need = mechDef.GetReservedSlots();

            if (used + need > total)
            {
                errors[MechValidationType.InvalidInventorySlots]
                    .Add($"RESERVED SLOTS: {need} criticals reserved, but have only {total - used}");
            }
        }

        public static bool ValidateAdd(MechComponentDef component, MechLabLocationWidget widget,
            bool current_result, ref string errorMessage, MechLabPanel mechlab)
        {
            if (!current_result)
                return false;

            var total = mechlab.activeMechDef.GetTotalSlots();
            var used = mechlab.activeMechDef.GetUsedSlots() + component.InventorySize;
            var need = mechlab.activeMechDef.GetReservedSlots() + (component as IDynamicSlots)?.ReserverdSlots ?? 0;

            if (component is ICategory cat_component && cat_component.CategoryDescriptor.AutoReplace)
            {
                var error = CategoryController.ValidateAdd(cat_component, widget, mechlab, out _, out _);
                if (error == CategoryError.MaximumReached || error == CategoryError.MaximumReachedLocation)
                {
                    var helper = new LocationHelper(widget);
                    var replace = helper.LocalInventory.FirstOrDefault(i =>
                        i.Def is ICategory c && c.CategoryID == cat_component.CategoryID);
                    if (replace != null)
                    {
                        used -= replace.Def.InventorySize;
                        if (replace.Def is IDynamicSlots slots)
                            need -= slots.ReserverdSlots;
                    }
                }
            }

            if (used + need <= total)
                return true;

            errorMessage = "Cannot Add - not enough free slots";
            return false;
        }

        internal static bool ValidateMechCanBeFielded(MechDef mechDef)
        {
            var total = mechDef.GetTotalSlots();
            var used = mechDef.GetUsedSlots();
            var need = mechDef.GetReservedSlots();
            return used + need <= total;
        }
    }
}