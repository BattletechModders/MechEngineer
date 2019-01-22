using System;
using System.Collections.Generic;
using System.Linq;
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

        public void PreProcess(object target, Dictionary<string, object> values)
        {


            if (!AutoAddCategoryIdIfMissing)
            {
                return;
            }

            if (!(target is MechComponentDef def))
            {
                return;
            }

            if (!IsMatch(def))
            {
                return;
            }

            if(Control.settings.AutoFixUpgradeDefSkip.Contains(def.Description.Id))
            {
                Control.mod.Logger.LogDebug("PreProcess: skipped " + def.Description.Id);
                return;
            }

            Control.mod.Logger.LogDebug("PreProcess: Fixing " + def.Description.Id );

            // TODO: copy structure from standard object

            values.TryGetValue("Custom", out var customObject);
            var custom = customObject as Dictionary<string, object>;
            if (custom == null)
            {
                custom = new Dictionary<string, object>();
                values["Custom"] = custom;
            }

            object create_new_category()
            {
                var category = new Dictionary<string, object>();
                category["CategoryID"] = CategoryId;
                return category;
            }

            // if category exists
            if (custom.TryGetValue("Category", out var categoryObject))
            {
                // if single category
                if (categoryObject is Dictionary<string, Object> catDictionary)
                {
                    // if correct category descreiption
                    if (catDictionary.TryGetValue("CategoryID", out var catID) && catID is string strID)
                    {
                        //if not same category
                        if (strID != CategoryId)
                        {
                            //turn to list and add new
                            var list = new List<object>();
                            list.Add(categoryObject);
                            list.Add(create_new_category());
                            custom["Category"] = list;
                        }
                    }
                    //replace with correct category
                    else
                        custom["Category"] = create_new_category();
                }
                //if category list
                else if (categoryObject is IEnumerable<object> catList)
                {
                    bool found = false;

                    var itemObjects = catList as object[] ?? catList.ToArray();
                    foreach (var item_object in itemObjects)
                    {
                        if (item_object is Dictionary<string, object> item_dict
                            && item_dict.TryGetValue("CategoryID", out var catID)
                            && catID is string strID
                            && strID == CategoryId)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var list = new List<object>();
                        list.Add(create_new_category());
                        list.AddRange(itemObjects);
                        custom["Category"] = list;
                    }
                }
                else
                    custom["Category"] = create_new_category();
            }
            else
            {
                custom["Category"] = create_new_category();
            }
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