using BattleTech;
using HBS.Collections;

namespace MechEngineer.Features.TagManager
{
    internal class TagManagerFeature : Feature<TagManagerSettings>
    {
        internal static TagManagerFeature Shared = new TagManagerFeature();

        internal override TagManagerSettings Settings => Control.settings.TagManager;

        internal override void SetupFeatureLoaded()
        {
            Settings.Setup();
        }

        internal void ManageComponentTags(MechComponentDef def)
        {
            var tags = def.ComponentTags;

            if (Settings.LostechStockWeaponVariantFix
                && def is WeaponDef
                && !def.Description.Id.EndsWith("-STOCK")
                && tags.Contains(MechValidationRules.ComponentTag_LosTech))
            {
                //Control.mod.Logger.LogDebug($"LostechStockWeaponVariantFix {def.Description.Id}");

                tags.Remove(MechValidationRules.ComponentTag_Stock);
                tags.Add(MechValidationRules.ComponentTag_Variant);
            }

            if (Check(tags, Settings.WhitelistComponentTagSet))
            {
                //Control.mod.Logger.LogDebug($"WhitelistComponentTags {def.Description.Id}");
                tags.Remove(MechValidationRules.Tag_Blacklisted);
            }

            if (Check(tags, Settings.BlacklistComponentTagSet))
            {
                //Control.mod.Logger.LogDebug($"BlacklistComponentTags {def.Description.Id}");
                tags.Add(MechValidationRules.Tag_Blacklisted);
            }
        }

        internal void ManageMechTags(MechDef def)
        {
            var tags = def.MechTags;

            if (Check(tags, Settings.WhitelistMechTagSet))
            {
                //Control.mod.Logger.LogDebug($"WhitelistMechTags {def.Description.Id}");
                tags.Remove(MechValidationRules.Tag_Blacklisted);
            }

            if (Check(tags, Settings.BlacklistMechTagSet))
            {
                //Control.mod.Logger.LogDebug($"BlacklistMechTags {def.Description.Id}");
                tags.Add(MechValidationRules.Tag_Blacklisted);
            }
        }

        bool Check(TagSet a, TagSet b)
        {
            if (a.Count < 1 || b.Count < 1)
            {
                return false;
            }
            return a.ContainsAny(b);
        }
    }
}