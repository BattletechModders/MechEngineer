using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    public class IdentityHelper : IIdentifier, IPreProcessor
    {
        public string Prefix { get; set; }
        public ChassisLocations AllowedLocations { get; set; }
        public ComponentType ComponentType { get; set; }
        public string CategoryId { get; set; }
        public bool AutoAddCategoryIdIfMissing { get; set; }

        public bool IsCustomType(MechComponentDef def)
        {
            return def.Is<Category>(out var category) && category.CategoryID == CategoryId;
        }

        public bool IsMatch(MechComponentDef def)
        {
            if (ComponentType != ComponentType.NotSet && def.ComponentType != ComponentType)
            {
                return false;
            }

            if (AllowedLocations != ChassisLocations.None)
            {
                if (!OnlyAllowedIn(def, AllowedLocations))
                {
                    return false;
                }
            }

            if (Prefix != null && !def.Description.Id.StartsWith(Prefix))
            {
                return false;
            }

            return true;
        }

        private static bool OnlyAllowedIn(MechComponentDef componentDef, ChassisLocations locations)
        {
            return (componentDef.AllowedLocations & locations) != 0 // def can be inserted in locations
                   && (componentDef.AllowedLocations & ~locations) == 0; // def can't be inserted anywhere outside of locations
        }

        public void PreProcess(MechComponentDef target, Dictionary<string, object> values)
        {
            if (!AutoAddCategoryIdIfMissing)
            {
                return;
            }

            if (!IsMatch(target))
            {
                return;
            }

            // TODO: copy structure from standard object

            values.TryGetValue("Custom", out var customObject);
            var custom = customObject as Dictionary<string, object>;
            if (custom == null)
            {
                custom = new Dictionary<string, object>();
                values["Custom"] = custom;
            }

            custom.TryGetValue("Category", out var categoryObject);
            var category = categoryObject as Dictionary<string, object>;
            if (category == null)
            {
                category = new Dictionary<string, object>();
                custom["Category"] = category;
            }

            if (category.ContainsKey("CategoryID"))
            {
                return;
            }

            category["CategoryID"] = CategoryId;
        }
    }

    internal class IdentityFuncHelper : IIdentifier
    {
        private readonly Func<MechComponentDef, bool> func;

        public IdentityFuncHelper(Func<MechComponentDef, bool> func)
        {
            this.func = func;
        }

        public bool IsCustomType(MechComponentDef def)
        {
            return func(def);
        }
    }
}