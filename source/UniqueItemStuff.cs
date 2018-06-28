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
            //new UniqueItem {ItemPrefix = "emod_armor_", Category = "Armor"},
            //new UniqueItem {ItemPrefix = "emod_structure_", Category = "Structure"},
            new UniqueItem {ItemPrefix = "emod_engine_", Category = "Engine Core"},
            new UniqueItem {ItemPrefix = "emod_engineslots_std_center", Category = "Engine Shielding"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_center", Category = "Engine Shielding"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_left", Category = "Engine Shielding Left"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_right", Category = "Engine Shielding Right"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_right", Category = "Engine Shielding Right"},
            new UniqueItem {ItemPrefix = "Gear_Gyro_", Category = "Gyro"},
            new UniqueItem {ItemPrefix = "Gear_Cockpit_", Category = "Cockpit"},
        };

        public UniqueCategory[] UniqueCategories =
        {
            //new UniqueCategory {Name = "Armor"},
            //new UniqueCategory {Name = "Structure"},
            new UniqueCategory {Name = "Engine Core"},
            new UniqueCategory {Name = "Engine Shielding"},
            new UniqueCategory {Name = "Gyro"},
            new UniqueCategory {Name = "Cockpit"},
            new UniqueCategory {Name = "Engine Shielding Left", Required = false},
            new UniqueCategory {Name = "Engine Shielding Right", Required = false},
        };
    }


    public class UniqueCategory
    {
        /// <summary>
        /// catefory Name
        /// </summary>
        public string Name;
        /// <summary>
        /// for validation checks - need this installed
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
        public string Category;

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
                if (inventory[i].ComponentRef.Def.IsUnique(out temp) && temp.Category == item.Category)
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