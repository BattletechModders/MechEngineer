using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    public partial class MechEngineerSettings
    {
        public UniqueItem[] Uniques =
        {
            //new UniqueItem {ItemPrefix = "emod_armor_", ReplaceTag = "Armor"},
            //new UniqueItem {ItemPrefix = "emod_structure_", ReplaceTag = "Structure"},
            new UniqueItem {ItemPrefix = "emod_engine_", ReplaceTag = "Engine Core"},
            new UniqueItem {ItemPrefix = "emod_engineslots_std_center", ReplaceTag = "Engine Shielding"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_center", ReplaceTag = "Engine Shielding"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_left", ReplaceTag = "Engine Shielding Left"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_right", ReplaceTag = "Engine Shielding Right"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_right", ReplaceTag = "Engine Shielding Right"},
            new UniqueItem {ItemPrefix = "Gear_Gyro_", ReplaceTag = "Gyro"},
            new UniqueItem {ItemPrefix = "Gear_Cockpit_", ReplaceTag = "Cockpit"},
        };

        public UniqueCategory[] UniqueCategories =
        {
            //new UniqueCategory {Tag = "Armor"},
            //new UniqueCategory {Tag = "Structure"},
            new UniqueCategory {Tag = "Engine Core"},
            new UniqueCategory {Tag = "Engine Shielding"},
            new UniqueCategory {Tag = "Gyro"},
            new UniqueCategory {Tag = "Cockpit"},
            new UniqueCategory {Tag = "Engine Shielding Left", Required = false},
            new UniqueCategory {Tag = "Engine Shielding Right", Required = false},
        };
    }


    public class UniqueCategory
    {
        /// <summary>
        /// catefory Name
        /// </summary>
        public string Tag;
        /// <summary>
        /// for validation checks - need this installed
        /// TODO: not implemented!
        /// </summary>
        public bool Required = true;
        /// <summary>
        /// Can this item be removed
        /// TODO: questionable, i cannot found a way to cancel item remove
        /// </summary>
        public bool CanRemove = true;


        public string ErrorMissing = "MISSING {0}: This mech must mount a {1}";
        public string ErrorToMany = "EXCESS {0}: This mech can't mount more then one {1}";
    }

    /// <summary>
    /// descriptor for unique component
    /// </summary>
    public class UniqueItem
    {
        /// <summary>
        /// prefix of item id
        /// </summary>
        public string ItemPrefix;
        /// <summary>
        /// catefory 
        /// </summary>
        public string ReplaceTag;

    }

    internal static partial class Extensions
    {
        public static bool IsUnique(this MechComponentDef componentDef)
        {
            if (componentDef == null)
                return false;

            return Control.settings.Uniques.Any(i => componentDef.Description.Id.StartsWith(i.ItemPrefix));
        }

        public static bool IsUnique(this MechComponentDef componentDef, out UniqueItem uniqueinfo)
        {
            uniqueinfo = null;

            if (componentDef == null)
                return false;

            uniqueinfo = Control.settings.Uniques.FirstOrDefault(i => componentDef.Description.Id.StartsWith(i.ItemPrefix));
            return uniqueinfo != null;
        }

        public static int FindUniqueItem(this List<MechLabItemSlotElement> inventory, UniqueItem item)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                UniqueItem temp;
                if (inventory[i].ComponentRef.Def.IsUnique(out temp) && temp.ReplaceTag == item.ReplaceTag)
                    return i;
            }

            return -1;
        }

        public static UniqueItem GetUniqueItem(this MechComponentRef componentRef)
        {
            if (componentRef == null || componentRef.Def == null)
                return null;

            UniqueItem item;
            return componentRef.Def.IsUnique(out item) ? item : null;
        }
    }
}